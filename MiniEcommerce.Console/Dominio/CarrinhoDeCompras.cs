namespace MiniEcommerce;

public sealed record ItemCarrinho(Produto Produto, int Quantidade)
{
    public decimal Subtotal => Produto.Preco * Quantidade;
}

/// <summary>O carrinho consulta estoque, mas só o checkout efetiva a baixa.</summary>
public sealed class CarrinhoDeCompras
{
    private readonly Dictionary<int, ItemCarrinho> _itens = [];

    public IReadOnlyList<ItemCarrinho> Itens => _itens.Values.OrderBy(item => item.Produto.Id).ToArray();
    public decimal Subtotal => _itens.Values.Sum(item => item.Subtotal);

    public void Adicionar(Produto produto, int quantidade)
    {
        ArgumentNullException.ThrowIfNull(produto);
        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve ser maior que zero.");

        _itens.TryGetValue(produto.Id, out var existente);
        if (existente is not null && !ReferenceEquals(existente.Produto, produto))
            throw new ArgumentException("O identificador pertence a outro produto.", nameof(produto));

        var quantidadeAtual = existente?.Quantidade ?? 0;
        // A subtração evita overflow ao somar quantidades muito grandes.
        if (quantidade > produto.QuantidadeEmEstoque - quantidadeAtual)
            throw new InvalidOperationException("Estoque insuficiente para a quantidade total no carrinho.");

        _itens[produto.Id] = new ItemCarrinho(produto, quantidadeAtual + quantidade);
    }

    public bool Remover(int produtoId) => _itens.Remove(produtoId);
}
