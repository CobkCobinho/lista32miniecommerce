using MiniEcommerce;

namespace MiniEcommerce.Tests;

public class CatalogoTests
{
    [Fact]
    public void IniciaSemProdutos()
    {
        Assert.Empty(new Catalogo().Listar());
    }

    [Fact]
    public void CadastraProdutosComIdsUnicosEListaSeusDados()
    {
        var catalogo = new Catalogo();
        var livro = catalogo.Cadastrar("Livro", 49.90m, 3);
        var caneta = catalogo.Cadastrar("Caneta", 2.50m, 10);

        Assert.Equal(new[] { 1, 2 }, catalogo.Listar().Select(produto => produto.Id));
        Assert.Same(livro, catalogo.ObterPorId(livro.Id));
        Assert.Same(caneta, catalogo.ObterPorId(caneta.Id));
        Assert.Equal("Livro", livro.Nome);
        Assert.Equal(49.90m, livro.Preco);
        Assert.Equal(3, livro.QuantidadeEmEstoque);
    }

    [Fact]
    public void CadastroInvalidoNaoAlteraCatalogoNemConsomeId()
    {
        var catalogo = new Catalogo();

        Assert.Throws<ArgumentOutOfRangeException>(() => catalogo.Cadastrar("Inválido", -1m, 1));
        Assert.Empty(catalogo.Listar());
        Assert.Equal(1, catalogo.Cadastrar("Válido", 10m, 1).Id);
    }

    [Fact]
    public void ConsultaDeIdInexistenteFalhaDeFormaClara()
    {
        Assert.Throws<KeyNotFoundException>(() => new Catalogo().ObterPorId(42));
    }

    [Fact]
    public void AlterarCopiaDaListagemNaoAlteraCatalogo()
    {
        var catalogo = new Catalogo();
        var produto = catalogo.Cadastrar("Livro", 10m, 1);
        var copia = catalogo.Listar().ToArray();
        copia[0] = new Produto(99, "Outro", 0m, 0);

        Assert.Same(produto, Assert.Single(catalogo.Listar()));
    }
}
