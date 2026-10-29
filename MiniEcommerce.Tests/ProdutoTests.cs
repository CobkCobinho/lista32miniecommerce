using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class ProdutoTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void NaoAceitaPrecoNegativo(int preco)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Produto(1, "Livro", preco, 1));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void NaoAceitaEstoqueNegativo(int estoque)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Produto(1, "Livro", 10m, estoque));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void NaoAceitaNomeVazio(string? nome)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Produto(1, nome!, 10m, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NaoAceitaIdentificadorInvalido(int id)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Produto(id, "Livro", 10m, 1));
    }

    [Fact]
    public void AceitaPrecoEEstoqueIguaisAZero()
    {
        var produto = new Produto(1, "  Brinde  ", 0m, 0);

        Assert.Equal(1, produto.Id);
        Assert.Equal("Brinde", produto.Nome);
        Assert.Equal(0m, produto.Preco);
        Assert.Equal(0, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public void PreservaPrecisaoMonetaria()
    {
        var produto = new Produto(1, "Caneta", 0.01m, 10);

        Assert.Equal(0.01m, produto.Preco);
        Assert.Equal(10, produto.QuantidadeEmEstoque);
    }
}
