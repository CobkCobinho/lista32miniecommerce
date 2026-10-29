using MiniEcommerce.Dominio;

namespace MiniEcommerce.Servicos;

public sealed class Catalogo
{
    private readonly List<Produto> _produtos = [];
    private int _proximoId = 1;

    public Produto Cadastrar(string nome, decimal preco, int quantidadeEmEstoque)
    {
        // Só consome o identificador depois de validar o cadastro.
        var produto = new Produto(_proximoId, nome, preco, quantidadeEmEstoque);
        _produtos.Add(produto);
        _proximoId++;
        return produto;
    }

    public IReadOnlyList<Produto> Listar() => _produtos.ToArray();

    public Produto ObterPorId(int id) =>
        _produtos.FirstOrDefault(produto => produto.Id == id)
        ?? throw new KeyNotFoundException("Produto não encontrado no catálogo.");
}
