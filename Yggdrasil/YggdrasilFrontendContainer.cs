using Yggdrasil.Protocols;

namespace Yggdrasil;

/// <summary>
/// The <see cref="IYggdrasilContainer"/> implementation for the <see cref="Side.Frontend"/> side.
/// </summary>
internal sealed class YggdrasilFrontendContainer : IYggdrasilContainer
{
    private readonly WireProtocol _protocolType;
    private readonly IWireProtocol _protocol;
    private readonly YggdrasilBridge _bridge;
    
    internal YggdrasilFrontendContainer(string url, WireProtocol protocolType)
    {
        _protocolType = protocolType;
        _protocol = protocolType switch
        {
            WireProtocol.HTTP => new HttpWireProtocol(),
            WireProtocol.WebSocket => new WebSocketWireProtocol(),
            _ => throw new ArgumentOutOfRangeException(nameof(protocolType), protocolType, "The given protocol is not supported.")
        };
        
        _protocol.InitializeAsFrontend(url);
        
        _bridge = new YggdrasilBridge(_protocol);
    }

    public void AddWire<T>() where T : class
    {
        if (_protocolType == WireProtocol.HTTP)
        {
            throw new NotSupportedException("The HTTP protocol does not support adding wires on the frontend.");
        }
        
        _bridge.AddWire<T>();
    }

    public T? GetWire<T>() where T : class
    {
        return _bridge.GetWire<T>();
    }

    public Task StartAsync() => _protocol.StartAsync();

    public Task StopAsync() => _protocol.StopAsync();
}