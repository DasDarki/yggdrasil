namespace Yggdrasil;

/// <summary>
/// The possible wire protocols supported by yggdrasil.
/// </summary>
public enum WireProtocol
{
    /// <summary>
    /// It uses the HTTP and REST protocol to communicate between the client and the server.
    /// </summary>
    /// <remarks>
    /// Because of the nature of the HTTP protocol, it is not possible to call methods on the client from the server.
    /// </remarks>
    HTTP,
    /// <summary>
    /// Enables the WebSocket protocol to communicate between the client and the server. This protocol allows
    /// bidirectional communication between the client and the server.
    ///
    /// This is the recommended protocol to use and the default protocol.
    /// </summary>
    WebSocket
}