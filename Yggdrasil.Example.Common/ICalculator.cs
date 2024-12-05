namespace Yggdrasil.Example.Common;

[Wire]
public interface ICalculator
{
    int Calcs { get; }
    
    int Add(int a, int b);
    
    int Subtract(int a, int b);
    
    int Multiply(int a, int b);
    
    int Divide(int a, int b);
}