namespace MiniEcommerce;

public sealed record ItemPedido(int ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade)
{
    public decimal Subtotal => throw new NotImplementedException();
}

public sealed class Pedido
{
    internal Pedido(IReadOnlyList<ItemPedido> itens) => throw new NotImplementedException();
    public Guid Id => throw new NotImplementedException();
    public DateTimeOffset DataCriacao => throw new NotImplementedException();
    public IReadOnlyList<ItemPedido> Itens => throw new NotImplementedException();
    public decimal Total => throw new NotImplementedException();
}
