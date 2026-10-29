using System.Globalization;
using MiniEcommerce.Dominio;
using MiniEcommerce.Servicos;

namespace MiniEcommerce.ConsoleApp;

internal static class Program
{
    private static void Main()
    {
        System.Console.InputEncoding = System.Text.Encoding.UTF8;
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        new AplicacaoConsole(System.Console.In, System.Console.Out).Executar();
    }
}

/// <summary>Menu interativo. TextReader/TextWriter permitem testar sem um terminal real.</summary>
public sealed class AplicacaoConsole
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");
    private readonly LeitorDeEntrada _leitor;
    private readonly TextWriter _saida;
    private readonly Catalogo _catalogo = new();
    private readonly CarrinhoDeCompras _carrinho = new();
    private readonly ServicoDeCheckout _checkout = new();

    public AplicacaoConsole(TextReader entrada, TextWriter saida)
    {
        ArgumentNullException.ThrowIfNull(entrada);
        ArgumentNullException.ThrowIfNull(saida);
        _leitor = new LeitorDeEntrada(entrada, saida);
        _saida = saida;
    }

    public void Executar()
    {
        while (true)
        {
            ExibirMenu();
            var opcao = _leitor.LerOpcao();
            if (opcao is null or "0")
            {
                _saida.WriteLine("Até logo!");
                return;
            }

            try
            {
                switch (opcao)
                {
                    case "1": CadastrarProduto(); break;
                    case "2": ExibirCatalogo(); break;
                    case "3": AdicionarAoCarrinho(); break;
                    case "4": ExibirCarrinho(); break;
                    case "5": RemoverDoCarrinho(); break;
                    case "6": FinalizarPedido(); break;
                    default: _saida.WriteLine("Opção inválida. Escolha uma opção do menu."); break;
                }
            }
            catch (EndOfStreamException)
            {
                _saida.WriteLine("Entrada encerrada. Até logo!");
                return;
            }
            catch (OverflowException)
            {
                _saida.WriteLine("Erro: os valores excedem o limite de cálculo. Revise as quantidades e preços.");
            }
            catch (Exception erro) when (erro is ArgumentException or InvalidOperationException or KeyNotFoundException)
            {
                _saida.WriteLine("Erro: " + erro.Message);
            }

            _saida.WriteLine();
        }
    }

    private void ExibirMenu()
    {
        _saida.WriteLine("====================================");
        _saida.WriteLine("          MINI E-COMMERCE");
        _saida.WriteLine("====================================");
        _saida.WriteLine("1 - Cadastrar produto");
        _saida.WriteLine("2 - Listar catálogo");
        _saida.WriteLine("3 - Adicionar produto ao carrinho");
        _saida.WriteLine("4 - Ver carrinho");
        _saida.WriteLine("5 - Remover item do carrinho");
        _saida.WriteLine("6 - Finalizar pedido");
        _saida.WriteLine("0 - Sair");
        _saida.Write("Escolha: ");
    }

    private void CadastrarProduto()
    {
        var nome = _leitor.LerTexto("Nome: ");
        var preco = _leitor.LerPreco("Preço (ex.: 19,90): ");
        var estoque = _leitor.LerInteiro("Quantidade em estoque: ");
        var produto = _catalogo.Cadastrar(nome, preco, estoque);
        _saida.WriteLine($"Produto cadastrado: #{produto.Id} - {produto.Nome}.");
    }

    private void ExibirCatalogo()
    {
        var produtos = _catalogo.Listar();
        if (produtos.Count == 0)
        {
            _saida.WriteLine("Catálogo vazio. Cadastre um produto na opção 1.");
            return;
        }

        _saida.WriteLine("--- CATÁLOGO ---");
        foreach (var produto in produtos)
            _saida.WriteLine($"#{produto.Id} | {produto.Nome} | {Dinheiro(produto.Preco)} | Estoque: {produto.QuantidadeEmEstoque}");
    }

    private void AdicionarAoCarrinho()
    {
        if (_catalogo.Listar().Count == 0)
        {
            ExibirCatalogo();
            return;
        }

        ExibirCatalogo();
        var produto = _catalogo.ObterPorId(_leitor.LerInteiro("ID do produto: "));
        var quantidade = _leitor.LerInteiro("Quantidade desejada: ");
        _carrinho.Adicionar(produto, quantidade);
        _saida.WriteLine("Produto adicionado ao carrinho.");
    }

    private void ExibirCarrinho()
    {
        if (_carrinho.Itens.Count == 0)
        {
            _saida.WriteLine("O carrinho está vazio.");
            return;
        }

        _saida.WriteLine("--- CARRINHO ---");
        foreach (var item in _carrinho.Itens)
            _saida.WriteLine($"#{item.Produto.Id} | {item.Produto.Nome} | {item.Quantidade} un. x {Dinheiro(item.Produto.Preco)} = {Dinheiro(item.Subtotal)}");
        _saida.WriteLine("Total do carrinho: " + Dinheiro(_carrinho.Subtotal));
    }

    private void RemoverDoCarrinho()
    {
        ExibirCarrinho();
        if (_carrinho.Itens.Count == 0)
            return;

        var id = _leitor.LerInteiro("ID do produto a remover: ");
        _saida.WriteLine(_carrinho.Remover(id) ? "Item removido do carrinho." : "Item não encontrado no carrinho.");
    }

    private void FinalizarPedido()
    {
        if (_carrinho.Itens.Count == 0)
            throw new InvalidOperationException("O carrinho está vazio.");

        ExibirCarrinho();
        var confirmacao = _leitor.LerTexto("Confirmar compra? [S/N]: ").Trim();
        if (!confirmacao.Equals("s", StringComparison.OrdinalIgnoreCase)
            && !confirmacao.Equals("sim", StringComparison.OrdinalIgnoreCase))
        {
            _saida.WriteLine("Checkout cancelado. Carrinho e estoque preservados.");
            return;
        }

        var pedido = _checkout.Finalizar(_carrinho);
        _saida.WriteLine("--- PEDIDO ---");
        _saida.WriteLine("Pedido finalizado: " + pedido.Id);
        _saida.WriteLine("Data: " + pedido.DataCriacao.ToLocalTime().ToString("dd/MM/yyyy HH:mm", Cultura));
        foreach (var item in pedido.Itens)
            _saida.WriteLine($"#{item.ProdutoId} | {item.NomeProduto} | {item.Quantidade} un. x {Dinheiro(item.PrecoUnitario)} = {Dinheiro(item.Subtotal)}");
        _saida.WriteLine("Total do pedido: " + Dinheiro(pedido.Total));
        _saida.WriteLine("Estoque atualizado e carrinho esvaziado.");
    }

    private static string Dinheiro(decimal valor) => valor.ToString("C2", Cultura);
}
