using System.Globalization;

namespace MiniEcommerce.ConsoleApp;

/// <summary>Centraliza leitura e conversão de entradas sem misturar regras de negócio.</summary>
internal sealed class LeitorDeEntrada(TextReader entrada, TextWriter saida)
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    public string? LerOpcao() => entrada.ReadLine()?.Trim();

    public string LerTexto(string mensagem)
    {
        saida.Write(mensagem);
        return entrada.ReadLine() ?? throw new EndOfStreamException();
    }

    public int LerInteiro(string mensagem)
    {
        if (int.TryParse(LerTexto(mensagem).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor))
            return valor;

        throw new ArgumentException("Informe um número inteiro válido.");
    }

    public decimal LerPreco(string mensagem)
    {
        var texto = LerTexto(mensagem).Trim();
        const NumberStyles estilos = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        if (decimal.TryParse(texto, estilos, Cultura, out var valor)
            || decimal.TryParse(texto, estilos, CultureInfo.InvariantCulture, out valor))
            return valor;

        throw new ArgumentException("Informe um preço válido, com vírgula ou ponto decimal e sem separador de milhar.");
    }
}
