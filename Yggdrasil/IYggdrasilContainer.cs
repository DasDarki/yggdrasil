namespace Yggdrasil;

/// <summary>
/// Defines the base functionalities of each yggdrasil container. An yggdrasil container is basically
/// the scaffold of the yggdrasil system. It is the entry point of the system and is responsible for
/// managing the lifecycle of the system.
/// </summary>
public interface IYggdrasilContainer
{
    /// <summary>
    /// Defines a wire through which communication between the client and the server is possible. The given
    /// type must implement an wireable interface. Wireable interfaces are interfaces that are marked with the
    /// <see cref="WireAttribute"/>.
    /// This is used to define the behavior when this side is being called.
    /// </summary>
    /// <typeparam name="T">The class implementing the wireable interface.</typeparam>
    /// <remarks>
    /// Call this method <b>BEFORE</b> calling <see cref="StartAsync"/>.
    /// </remarks>
    void AddWire<T>() where T : class;
    
    /// <summary>
    /// Returns the registered wire of the given type. The given type must be a wireable interface.
    /// Wireable interfaces are interfaces that are marked with the <see cref="WireAttribute"/>.
    /// This is used to call the other side.
    /// </summary>
    /// <typeparam name="T">The wanted wireable interface.</typeparam>
    /// <returns>
    /// The wire of the given type or <see langword="null"/> if no wire of the given type is registered.
    /// </returns>
    T? GetWire<T>() where T : class;
    
    /// <summary>
    /// Starts the yggdrasil system.
    /// </summary>
    Task StartAsync();
    
    /// <summary>
    /// Stops the yggdrasil system.
    /// </summary>
    Task StopAsync();
}