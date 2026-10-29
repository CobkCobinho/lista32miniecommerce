namespace MiniEcommerce.Dominio;

/// <summary>Produto com valores válidos desde sua criação.</summary>
public sealed class Produto
{
    public Produto(int id, string nome, decimal preco, int quantidadeEmEstoque)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "O identificador deve ser positivo.");
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        if (preco < 0)
            throw new ArgumentOutOfRangeException(nameof(preco), "O preço não pode ser negativo.");
        if (quantidadeEmEstoque < 0)
            throw new ArgumentOutOfRangeException(nameof(quantidadeEmEstoque), "O estoque não pode ser negativo.");

        Id = id;
        Nome = nome.Trim();
        Preco = preco;
        QuantidadeEmEstoque = quantidadeEmEstoque;
    }

    internal void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve ser maior que zero.");
        if (quantidade > QuantidadeEmEstoque)
            throw new InvalidOperationException("Estoque insuficiente para concluir a compra.");

        QuantidadeEmEstoque -= quantidade;
    }

    public int Id { get; }
    public string Nome { get; }
    public decimal Preco { get; }
    public int QuantidadeEmEstoque { get; private set; }
}
