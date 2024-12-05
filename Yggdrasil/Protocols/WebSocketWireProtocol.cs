using System.Text.Json.Nodes;

namespace Yggdrasil.Protocols;

public class WebSocketWireProtocol : IWireProtocol
{
    public event Func<string, JsonArray, object?>? OnWireableResourceRequested;
    public void InitializeAsFrontend(string url)
    {
        throw new NotImplementedException();
    }

    public void InitializeAsBackend(int port)
    {
        throw new NotImplementedException();
    }

    public Task StartAsync()
    {
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        throw new NotImplementedException();
    }

    public object? SendOverWire(string id, object?[] args, Type? returnValue)
    {
        throw new NotImplementedException();
    }
}