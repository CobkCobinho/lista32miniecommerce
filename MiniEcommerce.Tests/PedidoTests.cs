using MiniEcommerce.Dominio;
using MiniEcommerce.Servicos;

namespace MiniEcommerce.Tests;

public class PedidoTests
{
    private readonly ServicoDeCheckout _checkout = new();

    [Fact]
    public void NaoFinalizaCarrinhoVazio()
    {
        Assert.Throws<InvalidOperationException>(() => _checkout.Finalizar(new CarrinhoDeCompras()));
    }

    [Fact]
    public void NaoAceitaCarrinhoNulo()
    {
        Assert.Throws<ArgumentNullException>(() => _checkout.Finalizar(null!));
    }

    [Fact]
    public void FinalizaPedidoComResumoCorretoEBaixaDeTodosOsProdutos()
    {
        var livro = new Produto(1, "Livro", 19.90m, 5);
        var caneta = new Produto(2, "Caneta", 2.50m, 10);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(livro, 2);
        carrinho.Adicionar(caneta, 3);

        var pedido = _checkout.Finalizar(carrinho);

        Assert.NotEqual(Guid.Empty, pedido.Id);
        Assert.Equal(TimeSpan.Zero, pedido.DataCriacao.Offset);
        Assert.Equal(47.30m, pedido.Total);
        Assert.Equal(3, livro.QuantidadeEmEstoque);
        Assert.Equal(7, caneta.QuantidadeEmEstoque);
        Assert.Collection(pedido.Itens,
            item =>
            {
                Assert.Equal(1, item.ProdutoId);
                Assert.Equal("Livro", item.NomeProduto);
                Assert.Equal(19.90m, item.PrecoUnitario);
                Assert.Equal(2, item.Quantidade);
                Assert.Equal(39.80m, item.Subtotal);
            },
            item =>
            {
                Assert.Equal(2, item.ProdutoId);
                Assert.Equal(7.50m, item.Subtotal);
            });
    }

    [Fact]
    public void CheckoutLimpaCarrinhoEImpedeCompraDuplicada()
    {
        var produto = new Produto(1, "Livro", 10m, 5);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(produto, 2);

        _checkout.Finalizar(carrinho);

        Assert.Empty(carrinho.Itens);
        Assert.Equal(0m, carrinho.Subtotal);
        Assert.Throws<InvalidOperationException>(() => _checkout.Finalizar(carrinho));
        Assert.Equal(3, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public void CompraDeTodoEstoqueNaoPermiteNovaAdicao()
    {
        var produto = new Produto(1, "Livro", 10m, 2);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(produto, 2);

        _checkout.Finalizar(carrinho);

        Assert.Equal(0, produto.QuantidadeEmEstoque);
        Assert.Throws<InvalidOperationException>(() => carrinho.Adicionar(produto, 1));
    }

    [Fact]
    public void EstoqueInsuficienteEmUmItemNaoDebitaNenhumOutro()
    {
        var livro = new Produto(1, "Livro", 10m, 5);
        var caneta = new Produto(2, "Caneta", 2m, 1);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(livro, 2);
        carrinho.Adicionar(caneta, 1);
        var outraCompra = new CarrinhoDeCompras();
        outraCompra.Adicionar(caneta, 1);
        _checkout.Finalizar(outraCompra);

        Assert.Throws<InvalidOperationException>(() => _checkout.Finalizar(carrinho));
        Assert.Equal(5, livro.QuantidadeEmEstoque);
        Assert.Equal(0, caneta.QuantidadeEmEstoque);
        Assert.Equal(new[] { 1, 2 }, carrinho.Itens.Select(item => item.Produto.Id));
    }

    [Fact]
    public void RevalidaEstoqueMesmoDepoisDaAdicaoAoCarrinho()
    {
        var produto = new Produto(1, "Livro", 10m, 3);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(produto, 3);
        var outraCompra = new CarrinhoDeCompras();
        outraCompra.Adicionar(produto, 1);
        _checkout.Finalizar(outraCompra);

        Assert.Throws<InvalidOperationException>(() => _checkout.Finalizar(carrinho));
        Assert.Equal(2, produto.QuantidadeEmEstoque);
        Assert.Equal(3, Assert.Single(carrinho.Itens).Quantidade);
    }

    [Fact]
    public void OverflowNoTotalNaoAlteraEstoqueNemCarrinho()
    {
        var produto = new Produto(1, "Produto caro", decimal.MaxValue, 1);
        var outro = new Produto(2, "Outro", 1m, 1);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(produto, 1);
        carrinho.Adicionar(outro, 1);

        Assert.Throws<OverflowException>(() => _checkout.Finalizar(carrinho));
        Assert.Equal(1, produto.QuantidadeEmEstoque);
        Assert.Equal(1, outro.QuantidadeEmEstoque);
        Assert.Equal(new[] { 1, 2 }, carrinho.Itens.Select(item => item.Produto.Id));
    }

    [Fact]
    public void PedidoPreservaResumoMesmoAposOutraCompra()
    {
        var produto = new Produto(1, "Livro", 5m, 5);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(produto, 2);
        var primeiro = _checkout.Finalizar(carrinho);
        carrinho.Adicionar(produto, 1);
        var segundo = _checkout.Finalizar(carrinho);

        Assert.NotEqual(primeiro.Id, segundo.Id);
        Assert.Equal(10m, primeiro.Total);
        Assert.Equal(2, Assert.Single(primeiro.Itens).Quantidade);
        Assert.Equal(5m, segundo.Total);
        Assert.Equal(2, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public void ItemRemovidoNaoEhCobradoNemTemEstoqueBaixado()
    {
        var livro = new Produto(1, "Livro", 10m, 5);
        var caneta = new Produto(2, "Caneta", 2m, 3);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(livro, 2);
        carrinho.Adicionar(caneta, 1);
        carrinho.Remover(livro.Id);

        var pedido = _checkout.Finalizar(carrinho);

        Assert.Equal(2m, pedido.Total);
        Assert.Equal(2, Assert.Single(pedido.Itens).ProdutoId);
        Assert.Equal(5, livro.QuantidadeEmEstoque);
        Assert.Equal(2, caneta.QuantidadeEmEstoque);
    }

    [Fact]
    public void ProdutoGratuitoTambemTemBaixaDeEstoque()
    {
        var brinde = new Produto(1, "Brinde", 0m, 1);
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(brinde, 1);

        Assert.Equal(0m, _checkout.Finalizar(carrinho).Total);
        Assert.Equal(0, brinde.QuantidadeEmEstoque);
    }

    [Fact]
    public void TotalNaoSofreImprecisaoDePontoFlutuante()
    {
        var carrinho = new CarrinhoDeCompras();
        carrinho.Adicionar(new Produto(1, "A", 0.10m, 3), 3);
        carrinho.Adicionar(new Produto(2, "B", 0.20m, 2), 2);

        Assert.Equal(0.70m, _checkout.Finalizar(carrinho).Total);
    }
}
