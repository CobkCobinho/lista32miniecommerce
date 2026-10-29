using MiniEcommerce.ConsoleApp;

namespace MiniEcommerce.Tests;

public class ConsoleTests
{
    private static string Executar(params string[] linhas)
    {
        using var entrada = new StringReader(string.Join("\n", linhas) + (linhas.Length == 0 ? "" : "\n"));
        using var saida = new StringWriter();
        new AplicacaoConsole(entrada, saida).Executar();
        return saida.ToString();
    }

    [Fact]
    public void OpcaoZeroEncerraAplicacao()
    {
        Assert.Contains("Até logo", Executar("0"));
    }

    [Fact]
    public void FimDaEntradaEncerraSemLoopInfinito()
    {
        Assert.Contains("Até logo", Executar());
    }

    [Fact]
    public void OpcaoInvalidaExibeMensagemEPermiteContinuar()
    {
        var saida = Executar("99", "0");

        Assert.Contains("Opção inválida", saida);
        Assert.Contains("Até logo", saida);
    }

    [Fact]
    public void CatalogoVazioExibeOrientacao()
    {
        Assert.Contains("Catálogo vazio", Executar("2", "0"));
    }

    [Theory]
    [InlineData("19,90")]
    [InlineData("19.90")]
    public void CadastraEListaProdutoComVirgulaOuPontoDecimal(string preco)
    {
        var saida = Executar("1", "Livro", preco, "5", "2", "0");

        Assert.Contains("Produto cadastrado: #1", saida);
        Assert.Contains("Livro", saida);
        Assert.Contains("19,90", saida);
        Assert.Contains("Estoque: 5", saida);
    }

    [Fact]
    public void CadastroComPrecoNegativoEhRejeitado()
    {
        var saida = Executar("1", "Inválido", "-1", "2", "2", "0");

        Assert.Contains("preço não pode ser negativo", saida);
        Assert.Contains("Catálogo vazio", saida);
        Assert.DoesNotContain("Produto cadastrado:", saida);
    }

    [Fact]
    public void CadastroComEstoqueNegativoEhRejeitado()
    {
        var saida = Executar("1", "Inválido", "10", "-2", "2", "0");

        Assert.Contains("estoque não pode ser negativo", saida);
        Assert.Contains("Catálogo vazio", saida);
    }

    [Fact]
    public void QuantidadeNaoNumericaNaoDerrubaAplicacao()
    {
        var saida = Executar("1", "Livro", "10", "2", "3", "1", "abc", "4", "0");

        Assert.Contains("Informe um número inteiro", saida);
        Assert.Contains("O carrinho está vazio", saida);
    }

    [Fact]
    public void AdicionaERemoveLinhaDoCarrinhoSemBaixarEstoque()
    {
        var saida = Executar("1", "Livro", "10", "3", "3", "1", "2", "4", "5", "1", "4", "2", "0");

        Assert.Contains("Produto adicionado ao carrinho", saida);
        Assert.Contains("20,00", saida);
        Assert.Contains("Item removido", saida);
        Assert.Contains("O carrinho está vazio", saida);
        Assert.Contains("Estoque: 3", saida);
    }

    [Fact]
    public void NaoAdicionaQuantidadeSemEstoque()
    {
        var saida = Executar("1", "Livro", "10", "2", "3", "1", "3", "4", "0");

        Assert.Contains("Estoque insuficiente", saida);
        Assert.Contains("O carrinho está vazio", saida);
    }

    [Fact]
    public void CompraConfirmadaExibeResumoAtualizaEstoqueELimpaCarrinho()
    {
        var saida = Executar("1", "Livro", "10", "3", "3", "1", "2", "6", "s", "2", "4", "0");

        Assert.Contains("Pedido finalizado", saida);
        Assert.Contains("Total do pedido:", saida);
        Assert.Contains("20,00", saida);
        Assert.Contains("Estoque: 1", saida);
        Assert.Contains("O carrinho está vazio", saida);
    }

    [Fact]
    public void CancelarCheckoutPreservaCarrinhoEEstoque()
    {
        var saida = Executar("1", "Livro", "10", "3", "3", "1", "2", "6", "n", "2", "4", "0");

        Assert.Contains("Checkout cancelado", saida);
        Assert.Contains("Estoque: 3", saida);
        Assert.DoesNotContain("Pedido finalizado", saida);
        Assert.DoesNotContain("O carrinho está vazio", saida);
    }

    [Fact]
    public void CheckoutVazioNaoSolicitaConfirmacao()
    {
        var saida = Executar("6", "0");

        Assert.Contains("O carrinho está vazio", saida);
        Assert.DoesNotContain("Confirmar compra", saida);
    }

    [Fact]
    public void FimDaEntradaDuranteCadastroEncerraSemErro()
    {
        Assert.Contains("Entrada encerrada", Executar("1", "Livro"));
    }

    [Fact]
    public void IdentificadorInexistenteExibeErroEPermiteContinuar()
    {
        var saida = Executar("1", "Livro", "10", "3", "3", "99", "0");

        Assert.Contains("Produto não encontrado", saida);
        Assert.Contains("Até logo", saida);
    }

    [Fact]
    public void SeparadoresDeMilharSaoRejeitadosSemAmbiguidade()
    {
        var saida = Executar("1", "Livro", "1.234,56", "0");

        Assert.Contains("Informe um preço válido", saida);
        Assert.DoesNotContain("Produto cadastrado", saida);
    }
    [Fact]
    public void PermiteRemoverItemMesmoQuandoSubtotalExcedeLimiteDecimal()
    {
        var precoMaximo = decimal.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var saida = Executar("1", "Produto caro", precoMaximo, "2", "3", "1", "2", "5", "1", "4", "0");

        Assert.Contains("Item removido", saida);
        Assert.Contains("O carrinho está vazio", saida);
    }
}
