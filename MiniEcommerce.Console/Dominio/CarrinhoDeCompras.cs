namespace MiniEcommerce;

public sealed record ItemCarrinho(Produto Produto, int Quantidade)
{
    public decimal Subtotal => throw new NotImplementedException();
}

public sealed class CarrinhoDeCompras
{
    public IReadOnlyList<ItemCarrinho> Itens => throw new NotImplementedException();
    public decimal Subtotal => throw new NotImplementedException();
    public void Adicionar(Produto produto, int quantidade) => throw new NotImplementedException();
    public bool Remover(int produtoId) => throw new NotImplementedException();
}
