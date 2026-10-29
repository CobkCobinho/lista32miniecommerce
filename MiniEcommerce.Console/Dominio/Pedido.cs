namespace MiniEcommerce.Dominio;

/// <summary>Cópia dos dados financeiros no momento da compra.</summary>
public sealed record ItemPedido(int ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade)
{
    public decimal Subtotal => PrecoUnitario * Quantidade;
}

public sealed class Pedido
{
    internal Pedido(IReadOnlyList<ItemPedido> itens)
    {
        ArgumentNullException.ThrowIfNull(itens);
        if (itens.Count == 0)
            throw new ArgumentException("Um pedido deve possuir itens.", nameof(itens));

        Itens = Array.AsReadOnly(itens.ToArray());
        // Calcula tudo antes de qualquer baixa de estoque.
        Total = Itens.Sum(item => item.Subtotal);
        Id = Guid.NewGuid();
        DataCriacao = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }
    public DateTimeOffset DataCriacao { get; }
    public IReadOnlyList<ItemPedido> Itens { get; }
    public decimal Total { get; }
}
