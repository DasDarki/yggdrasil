using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Yggdrasil.Protocols;

namespace Yggdrasil;

/// <summary>
/// The yggdrasil bridge bridges the data flow between the client and server communication.
/// </summary>
internal class YggdrasilBridge
{
    private readonly IWireProtocol _protocol;
    private readonly ConcurrentDictionary<string, Func<JsonArray, object?>> _wires = new();
    
    internal YggdrasilBridge(IWireProtocol protocol)
    {
        _protocol = protocol;
        _protocol.OnWireableResourceRequested += OnWireRequest;
    }

    internal void AddWire<T>() where T : class
    {
        var instanceType = typeof(T);
        var instance = Activator.CreateInstance(instanceType);
        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to create instance of {instanceType}");
        }
        
        foreach (var interfaceType in instanceType.GetInterfaces())
        {
            var wireAttribute = interfaceType.GetCustomAttribute<WireAttribute>();
            if (wireAttribute == null)
            {
                continue;
            }
            
            foreach (var method in interfaceType.GetMethods())
            {
                AddWireMethod(interfaceType, instance, method);
            }
        }
    }
    
    internal T GetWire<T>() where T : class
    {
        var interfaceType = typeof(T);
        if (!interfaceType.IsInterface)
        {
            throw new InvalidOperationException($"{interfaceType} is not an interface");
        }

        var resource = WireableResourceBuilder.BuildWireableResource(interfaceType, _protocol);
        if (resource is T wire)
        {
            return wire;
        }
        
        throw new InvalidOperationException($"Failed to cast {resource} to {interfaceType}");
    }

    private object? OnWireRequest(string id, JsonArray args)
    {
        if (!_wires.TryGetValue(id, out var wire))
        {
            throw new Exception($"wire {id} not found");
        }
        
        return wire(args);
    }
    
    private void AddWireMethod(Type interfaceType, object instance, MethodInfo method)
    {
        var id = $"{interfaceType.FullName}:{method.Name}";
        _wires[id] = args =>
        {
            var parameters = method.GetParameters();
            if (parameters.Length != args.Count)
            {
                throw new InvalidOperationException($"Expected {parameters.Length} arguments, but got {args.Count}");
            }
            
            var methodArgs = new object?[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var arg = args[i];
                if (arg == null || arg.GetValueKind() == JsonValueKind.Null)
                {
                    methodArgs[i] = null;
                }
                else
                {
                    if (parameter.ParameterType == typeof(JsonElement))
                    {
                        methodArgs[i] = arg;
                    }
                    else
                    {
                        methodArgs[i] = JsonSerializer.Deserialize(arg.ToString(), parameter.ParameterType);
                    }
                }
            }
            
            return method.Invoke(instance, methodArgs);
        };
    }
}