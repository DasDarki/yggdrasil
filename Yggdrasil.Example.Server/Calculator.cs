using Yggdrasil.Example.Common;

namespace Yggdrasil.Example.Server;

public class Calculator : ICalculator
{
    public int Calcs { get; private set; }
    
    public int Add(int a, int b)
    {
        Calcs++;
        return a + b;
    }

    public Task<int> SubtractAsync(int a, int b)
    {
        Calcs++;
        return Task.FromResult(a - b);
    }

    public int Multiply(int a, int b)
    {
        Calcs++;
        return a * b;
    }

    public int Divide(int a, int b)
    {
        Calcs++;
        return a / b;
    }
}