using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WatsonWebserver;
using WatsonWebserver.Core;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Yggdrasil.Protocols;

internal sealed class HttpWireProtocol : IWireProtocol
{
    public event Func<string, JsonArray, object?>? OnWireableResourceRequested;

    private bool _isFrontend;
    private Webserver _backendServer = null!;
    private HttpClient _httpClient = null!;
    
    public void InitializeAsFrontend(string url)
    {
        _isFrontend = true;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(url);
    }

    public void InitializeAsBackend(int port)
    {
        _isFrontend = false;
        _backendServer = new Webserver(new WebserverSettings("127.0.0.1", port), ctx =>
        {
            ctx.Response.StatusCode = 404;
            ctx.Response.ContentType = "text/plain";
            return ctx.Response.Send("Not found.");
        });
        
        _backendServer.Routes.PreAuthentication.Parameter.Add(HttpMethod.POST, "/.well-known/@yggdrasil/{id}/wire", OnHandleWireRequestAsync);
    }

    public Task StartAsync()
    {
        return _isFrontend ? Task.CompletedTask : Task.Run(() => _backendServer.Start());
    }

    public Task StopAsync()
    {
        if (!_isFrontend) return Task.Run(() => _backendServer.Stop());
        
        _httpClient.Dispose();
        return Task.CompletedTask;

    }

    public object? SendOverWire(string id, object?[] args, Type? returnValue)
    {
        if (!_isFrontend)
        {
            throw new InvalidOperationException("Cannot send over wire from backend when using HTTP protocol.");
        }

        var jargs = new JsonArray();
        foreach (var arg in args)
        {
            jargs.Add(arg);
        }
        
        var res = _httpClient.PostAsync($"/.well-known/@yggdrasil/{id}/wire", new StringContent(jargs.ToJsonString(), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
        res.EnsureSuccessStatusCode();

        try
        {
            return returnValue == null ? null : JsonSerializer.Deserialize(res.Content.ReadAsStream(), returnValue);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize response: '" + res.Content.ReadAsStringAsync().GetAwaiter().GetResult() + "'", ex);
        }
    }

    private async Task OnHandleWireRequestAsync(HttpContextBase ctx)
    {
        try
        {
            var id = ctx.Request.Url.Parameters["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/plain";
                await ctx.Response.Send("wire id is missing.");
                return;
            }
            
            var content = ctx.Request.DataAsString;
            if (string.IsNullOrWhiteSpace(content))
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/plain";
                await ctx.Response.Send("wire content is missing.");
                return;
            }

            if (JsonNode.Parse(content) is not JsonArray args)
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/plain";
                await ctx.Response.Send("wire content is not valid.");
                return;
            }
            
            var result = OnWireableResourceRequested?.Invoke(id, args);
            if (result is Task task)
            {
                await task;
                var property = task.GetType().GetProperty("Result");
                result = property?.GetValue(task);
            }
            
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(result?.ToString() ?? "null");
        }
        catch (Exception e)
        {
            if (e.Message.Contains("not found"))
            {
                ctx.Response.StatusCode = 404;
                ctx.Response.ContentType = "text/plain";
                await ctx.Response.Send("Wireable resource not found.");
                return;
            }
            
            Console.WriteLine(e);
            
            ctx.Response.StatusCode = 500;
            ctx.Response.ContentType = "text/plain";
            await ctx.Response.Send("Internal server error.");
        }
    }
}