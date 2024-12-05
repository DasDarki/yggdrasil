using System.Reflection;
using System.Reflection.Emit;
using Sigil.NonGeneric;
using Yggdrasil.Protocols;

namespace Yggdrasil;

/// <summary>
/// Utility class to dynamically generate a wireable resource from a provided interface using Sigil.
/// </summary>
internal static class WireableResourceBuilder
{
    internal static object? BuildWireableResource(Type interfaceType, IWireProtocol protocol)
    {
        if (!interfaceType.IsInterface)
            throw new ArgumentException("The provided type must be an interface.", nameof(interfaceType));

        var typeBuilder = CreateTypeBuilder($"{interfaceType.Name}_Proxy");
        typeBuilder.AddInterfaceImplementation(interfaceType);
            
        var protocolField = typeBuilder.DefineField(
            "_protocol",
            typeof(IWireProtocol),
            FieldAttributes.Private);
            
        var constructorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            [typeof(IWireProtocol)]);
            
        var ctorIl = constructorBuilder.GetILGenerator();
        ctorIl.Emit(OpCodes.Ldarg_0);
        ctorIl.Emit(OpCodes.Ldarg_1);
        ctorIl.Emit(OpCodes.Stfld, protocolField);
        ctorIl.Emit(OpCodes.Ret);

        foreach (var method in interfaceType.GetMethods())
        {
            var parameters = method.GetParameters();
            var paramterTypes = parameters.Select(p => p.ParameterType).ToArray();
                
            var emitter = Emit.BuildInstanceMethod(
                method.ReturnType,
                paramterTypes,
                typeBuilder,
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual);
                
            emitter.LoadArgument(0);
            emitter.LoadField(protocolField);
                
            emitter.LoadConstant(interfaceType.FullName + ":" + method.Name);
                
            emitter.LoadConstant(paramterTypes.Length);
            emitter.NewArray<object>();
                
            for (var i = 0; i < paramterTypes.Length; i++)
            {
                emitter.Duplicate();
                emitter.LoadConstant(i);
                emitter.LoadArgument((ushort)(i + 1));
                emitter.Box(paramterTypes[i]);
                emitter.StoreElement<object>();
            }
                
            emitter.LoadConstant(method.ReturnType);
            emitter.Call(typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
            emitter.CallVirtual(typeof(IWireProtocol).GetMethod(nameof(IWireProtocol.SendOverWire)));
                
            if (method.ReturnType == typeof(void))
            {
                emitter.Pop();
            }
            else
            {
                if (method.ReturnType.IsValueType)
                {
                    emitter.UnboxAny(method.ReturnType);
                }
                else
                {
                    emitter.CastClass(method.ReturnType);
                }
            }
                
            emitter.Return();
            emitter.CreateMethod();
        }

        var dynamicType = typeBuilder.CreateType();
        return Activator.CreateInstance(dynamicType, protocol);
    }

    private static TypeBuilder CreateTypeBuilder(string typeName)
    {
        var assemblyName = new AssemblyName("DynamicWireableResourceAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicWireableResourceModule");

        return moduleBuilder.DefineType(
            typeName,
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);
    }
}