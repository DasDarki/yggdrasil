namespace Yggdrasil;

/// <summary>
/// The wire attribute marks an interface as a wire interface. A wire interface is an interface that is used
/// to communicate between the client and the server. Each method and property of the interface is a wire method
/// or property.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class WireAttribute : Attribute
{
    /// <summary>
    /// If set, it contains an array of roles that are allowed to call this method. If just empty, the user must be
    /// at least authenticated.
    /// </summary>
    public string[]? Authenticated { get; set; }
}