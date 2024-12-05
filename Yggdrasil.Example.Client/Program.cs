using Yggdrasil;
using Yggdrasil.Example.Common;

var connector = YggdrasilBootstrap.CreateFrontendConnector("http://127.0.0.1:25319", WireProtocol.HTTP);
await connector.StartAsync();

var calculator = connector.GetWire<ICalculator>();

Console.WriteLine("Current Calcs: " + calculator.Calcs);

Console.WriteLine(calculator.Add(1, 2));

Console.WriteLine("Current Calcs: " + calculator.Calcs);

Console.ReadKey();
