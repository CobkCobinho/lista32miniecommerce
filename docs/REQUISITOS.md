# Atendimento aos requisitos da Lista 32

## Estrutura e Git

| Requisito | Implementação / evidência |
| --- | --- |
| Repositório público | CobkCobinho/lista32miniecommerce. |
| Solução MiniEcommerce em formato .sln | MiniEcommerce.sln. |
| Projeto principal de console | MiniEcommerce.Console/MiniEcommerce.Console.csproj. |
| Projeto xUnit | MiniEcommerce.Tests/MiniEcommerce.Tests.csproj. |
| Projetos adicionados à solução | Ambos constam na solução. |
| Referência dos testes ao projeto principal | ProjectReference no projeto de testes. |
| Commit inicial da estrutura com Conventional Commits | chore: initial project structure with console and xunit apps. |
| Testes antes das funcionalidades | Pares test/feat documentados em TDD.md. |
| Refatoração separada | refactor(core): improve code readability and organize namespaces. |
| Documentação separada | docs: add comprehensive instructions in README.md. |
| Entrega na branch main | Código final e histórico publicados no repositório indicado. |
| bin, obj e dados locais fora do Git | .gitignore e .gitattributes. |

## Requisitos funcionais

| Item do enunciado | Código | Testes |
| --- | --- | --- |
| Nome, preço e estoque dos produtos | Dominio/Produto.cs | ProdutoTests. |
| Não permitir preço ou estoque negativos | Construtor de Produto | ProdutoTests e ConsoleTests. |
| Gerenciar catálogo | Servicos/Catalogo.cs | CatalogoTests. |
| Adicionar produtos com quantidade | Dominio/CarrinhoDeCompras.cs | CarrinhoDeComprasTests. |
| Validar estoque, inclusive adições repetidas | CarrinhoDeCompras.Adicionar | CarrinhoDeComprasTests. |
| Remover itens | CarrinhoDeCompras.Remover e opção 5 | CarrinhoDeComprasTests e ConsoleTests. |
| Calcular subtotal e total | ItemCarrinho, ItemPedido e Pedido | CarrinhoDeComprasTests e PedidoTests. |
| Baixar estoque após confirmar a compra | ServicoDeCheckout.Finalizar e opção 6 | PedidoTests e ConsoleTests. |
| Revalidar todos os itens antes da baixa | ServicoDeCheckout.Finalizar | PedidoTests, incluindo indisponibilidade de um dos itens. |
| Exibir preços e estoques atuais | Opção 2 em Program.cs | ConsoleTests e execução real do terminal. |
| Menu interativo no Program.cs | AplicacaoConsole | ConsoleTests. |
| README com arquitetura e execução | README.md | Instruções conferidas e comandos executados. |

Os caminhos de código da tabela são relativos a MiniEcommerce.Console.

## Sequência de desenvolvimento

1. Estrutura de solução e projetos.
2. Testes vermelhos e implementação de produto/catálogo.
3. Testes vermelhos e implementação do carrinho.
4. Testes vermelhos e implementação do checkout.
5. Testes vermelhos e implementação do console.
6. Refatoração sem mudar o comportamento.
7. Teste e correção de um caso limite de remoção.
8. Verificação Release, execução real do programa e documentação.

As informações administrativas do enunciado (prazo, composição da equipe e entrega individual no ambiente da disciplina) dependem do aluno e da instituição. Nenhum nome de integrante ou prazo foi inventado.
