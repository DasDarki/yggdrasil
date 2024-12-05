using Yggdrasil;
using Yggdrasil.Example.Server;

var container = YggdrasilBootstrap.CreateBackendServer(25319, WireProtocol.HTTP);

container.AddWire<Calculator>();

await container.StartAsync();

Console.ReadKey();

await container.StopAsync();