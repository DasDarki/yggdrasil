using System.Text.Json.Nodes;

namespace Yggdrasil.Protocols;

/// <summary>
/// The base interface for all protocols. See <see cref="WireProtocol"/> for more information.
/// </summary>
public interface IWireProtocol
{
    /// <summary>
    /// Should be called when a wireable resource is requested.
    /// </summary>
    event Func<string, JsonArray, object?> OnWireableResourceRequested; 

    /// <summary>
    /// Tells the protocol to initialize as a frontend connector.
    /// </summary>
    /// <param name="url">The url the frontend should connect to.</param>
    void InitializeAsFrontend(string url);
    
    /// <summary>
    /// Tells the protocol to initialize as a backend server.
    /// </summary>
    /// <param name="port">The port the backend should listen on.</param>
    void InitializeAsBackend(int port);
    
    /// <summary>
    /// Starts the underlying protocol.
    /// </summary>
    Task StartAsync();
    
    /// <summary>
    /// Stops the underlying protocol.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Sends the given ID and arguments over the wire and returns the response.
    /// </summary>
    /// <param name="id">The ID of the wireable resource to call.</param>
    /// <param name="args">The arguments to pass to the wireable resource.</param>
    /// <param name="returnValue">The type of the return value.</param>
    /// <returns>The response of the wireable resource.</returns>
    object? SendOverWire(string id, object?[] args, Type? returnValue);
}