using MiniEcommerce.Dominio;

namespace MiniEcommerce.Tests;

public class CarrinhoDeComprasTests
{
    [Fact]
    public void IniciaVazioEComSubtotalZero()
    {
        var carrinho = new CarrinhoDeCompras();

        Assert.Empty(carrinho.Itens);
        Assert.Equal(0m, carrinho.Subtotal);
    }

    [Fact]
    public void AdicionaProdutoECalculaSubtotalSemBaixarEstoque()
    {
        var produto = new Produto(1, "Livro", 19.90m, 5);
        var carrinho = new CarrinhoDeCompras();

        carrinho.Adicionar(produto, 2);

        var item = Assert.Single(carrinho.Itens);
        Assert.Same(produto, item.Produto);
        Assert.Equal(2, item.Quantidade);
        Assert.Equal(39.80m, item.Subtotal);
        Assert.Equal(39.80m, carrinho.Subtotal);
        Assert.Equal(5, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public void RepetirProdutoSomaQuantidadesNaMesmaLinha()
    {
        var produto = new Produto(1, "Livro", 10m, 5);
        var carrinho = new CarrinhoDeCompras();

        carrinho.Adicionar(produto, 2);
        carrinho.Adicionar(produto, 3);

        Assert.Equal(5, Assert.Single(carrinho.Itens).Quantidade);
        Assert.Equal(50m, carrinho.Subtotal);
    }

    [Fact]
    public void SomaSubtotalDeProdutosDiferentesComPrecisaoDecimal()
    {
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(new Produto(1, "Caneta", 0.10m, 10), 3);
        carrinho.Adicionar(new Produto(2, "Caderno", 8.75m, 2), 2);

        Assert.Equal(17.80m, carrinho.Subtotal);
        Assert.Equal(new[] { 1, 2 }, carrinho.Itens.Select(item => item.Produto.Id));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void NaoAceitaQuantidadeNaoPositiva(int quantidade)
    {
        var carrinho = new CarrinhoDeCompras();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            carrinho.Adicionar(new Produto(1, "Livro", 10m, 5), quantidade));
        Assert.Empty(carrinho.Itens);
    }

    [Fact]
    public void NaoAdicionaQuantidadeSuperiorAoEstoque()
    {
        var carrinho = new CarrinhoDeCompras();
        var produto = new Produto(1, "Livro", 10m, 2);

        Assert.Throws<InvalidOperationException>(() => carrinho.Adicionar(produto, 3));
        Assert.Empty(carrinho.Itens);
        Assert.Equal(2, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public void ValidaEstoqueContraQuantidadeAcumulada()
    {
        var carrinho = new CarrinhoDeCompras();
        var produto = new Produto(1, "Livro", 10m, 3);
        carrinho.Adicionar(produto, 2);

        Assert.Throws<InvalidOperationException>(() => carrinho.Adicionar(produto, 2));
        Assert.Equal(2, Assert.Single(carrinho.Itens).Quantidade);
        Assert.Equal(20m, carrinho.Subtotal);
    }

    [Fact]
    public void NaoAdicionaProdutoSemEstoque()
    {
        var carrinho = new CarrinhoDeCompras();

        Assert.Throws<InvalidOperationException>(() =>
            carrinho.Adicionar(new Produto(1, "Livro", 10m, 0), 1));
        Assert.Empty(carrinho.Itens);
    }

    [Fact]
    public void AceitaQuantidadeExatamenteIgualAoEstoqueSemOverflow()
    {
        var carrinho = new CarrinhoDeCompras();
        var produto = new Produto(1, "Brinde", 0m, int.MaxValue);
        carrinho.Adicionar(produto, int.MaxValue);

        Assert.Throws<InvalidOperationException>(() => carrinho.Adicionar(produto, 1));
        Assert.Equal(int.MaxValue, Assert.Single(carrinho.Itens).Quantidade);
    }

    [Fact]
    public void NaoAceitaProdutoNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new CarrinhoDeCompras().Adicionar(null!, 1));
    }

    [Fact]
    public void NaoMisturaObjetosDiferentesComMesmoIdentificador()
    {
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(new Produto(1, "Livro", 10m, 5), 1);

        Assert.Throws<ArgumentException>(() =>
            carrinho.Adicionar(new Produto(1, "Outro livro", 20m, 5), 1));
        Assert.Equal(10m, carrinho.Subtotal);
    }

    [Fact]
    public void RemoveLinhaInteiraERecalculaSubtotalSemAlterarEstoque()
    {
        var livro = new Produto(1, "Livro", 10m, 3);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(livro, 2);
        carrinho.Adicionar(new Produto(2, "Caneta", 2m, 10), 3);

        Assert.True(carrinho.Remover(livro.Id));
        Assert.Equal(2, Assert.Single(carrinho.Itens).Produto.Id);
        Assert.Equal(6m, carrinho.Subtotal);
        Assert.Equal(3, livro.QuantidadeEmEstoque);
    }

    [Fact]
    public void RemoverProdutoInexistenteRetornaFalse()
    {
        Assert.False(new CarrinhoDeCompras().Remover(99));
    }

    [Fact]
    public void RemoveUltimoItemEVoltaAoSubtotalZero()
    {
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(new Produto(1, "Livro", 10m, 1), 1);

        Assert.True(carrinho.Remover(1));
        Assert.Empty(carrinho.Itens);
        Assert.Equal(0m, carrinho.Subtotal);
    }
}
