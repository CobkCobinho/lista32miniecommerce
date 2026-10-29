namespace MiniEcommerce;

public sealed class Catalogo
{
    public Produto Cadastrar(string nome, decimal preco, int quantidadeEmEstoque)
        => throw new NotImplementedException();

    public IReadOnlyList<Produto> Listar() => throw new NotImplementedException();
    public Produto ObterPorId(int id) => throw new NotImplementedException();
}
