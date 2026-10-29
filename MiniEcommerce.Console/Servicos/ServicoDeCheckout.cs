using MiniEcommerce.Dominio;

namespace MiniEcommerce.Servicos;

public sealed class ServicoDeCheckout
{
    public Pedido Finalizar(CarrinhoDeCompras carrinho)
    {
        ArgumentNullException.ThrowIfNull(carrinho);
        var itens = carrinho.Itens;
        if (itens.Count == 0)
            throw new InvalidOperationException("O carrinho está vazio.");

        // Revalida todos os itens antes de alterar qualquer produto.
        foreach (var item in itens)
        {
            if (item.Quantidade > item.Produto.QuantidadeEmEstoque)
                throw new InvalidOperationException(
                    $"Estoque insuficiente para '{item.Produto.Nome}'. Revise o carrinho.");
        }

        var pedido = new Pedido(itens.Select(item => new ItemPedido(
            item.Produto.Id, item.Produto.Nome, item.Produto.Preco, item.Quantidade)).ToArray());

        foreach (var item in itens)
            item.Produto.BaixarEstoque(item.Quantidade);

        carrinho.Limpar();
        return pedido;
    }
}
