namespace Yggdrasil;

/// <summary>
/// The yggdrasil bootstrap takes care of starting the yggdrasil system. It uses the right backend
/// based on the side the project should running on.
/// </summary>
public static class YggdrasilBootstrap
{
    /// <summary>
    /// Creates the frontend connector container for the given hostname and port.
    /// </summary>
    /// <param name="url">The url to the server. This must contain the protocol, hostname and port. Do not include the path.</param>
    /// <param name="protocol">The protocol yggdrasil should use.</param>
    /// <returns>The frontend yggdrasil connector container.</returns>
    public static IYggdrasilContainer CreateFrontendConnector(string url, WireProtocol protocol = WireProtocol.WebSocket)
    {
        return new YggdrasilFrontendContainer(url, protocol);
    }
    
    /// <summary>
    /// Creates the backend server container for the given port.
    /// </summary>
    /// <param name="port">The port the server should listen on.</param>
    /// <param name="protocol">The protocol yggdrasil should use.</param>
    /// <returns>The backend yggdrasil server container.</returns>
    public static IYggdrasilContainer CreateBackendServer(int port, WireProtocol protocol = WireProtocol.WebSocket)
    {
        return new YggdrasilBackendContainer(port, protocol);
    }
}