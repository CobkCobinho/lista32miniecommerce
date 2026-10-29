namespace MiniEcommerce.ConsoleApp;

internal static class Program
{
    private static void Main()
    {
        System.Console.InputEncoding = System.Text.Encoding.UTF8;
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        new AplicacaoConsole(System.Console.In, System.Console.Out).Executar();
    }
}

public sealed class AplicacaoConsole
{
    public AplicacaoConsole(TextReader entrada, TextWriter saida) { }
    public void Executar() => throw new NotImplementedException();
}
