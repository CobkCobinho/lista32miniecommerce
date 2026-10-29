# MiniEcommerce - Lista 32

Aplicação de console em **C# / .NET 10**, desenvolvida para praticar regras de negócio, testes xUnit, TDD e Conventional Commits.

## Funcionalidades

- Cadastrar produtos com identificador, nome, preço e estoque.
- Impedir nomes vazios, preços negativos e estoque negativo.
- Consultar o catálogo com preços e quantidades atuais.
- Adicionar produtos ao carrinho, somando quantidades do mesmo produto.
- Rejeitar quantidades inválidas ou superiores ao estoque disponível.
- Consultar e remover itens do carrinho.
- Confirmar ou cancelar o checkout.
- Calcular o pedido e baixar o estoque somente após a confirmação.
- Exibir o resumo financeiro e esvaziar o carrinho depois da compra.

Os dados ficam **em memória durante a execução**. Ao sair, o catálogo e o carrinho são descartados. Não há banco de dados, pagamento real ou dependência de um servidor.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0), e não apenas o runtime.
- Git para clonar o repositório.
- Acesso à internet na primeira restauração dos pacotes xUnit.

O arquivo `global.json` aceita SDKs da linha 10.0 por meio de `rollForward: latestFeature`. A implementação foi validada com o SDK 10.0.401.

## Executar

```sh
git clone https://github.com/CobkCobinho/lista32miniecommerce.git
cd lista32miniecommerce
dotnet restore MiniEcommerce.sln
dotnet run --project MiniEcommerce.Console/MiniEcommerce.Console.csproj
```

### Menu

```text
1 - Cadastrar produto
2 - Listar catálogo
3 - Adicionar produto ao carrinho
4 - Ver carrinho
5 - Remover item do carrinho
6 - Finalizar pedido
0 - Sair
```

Preços aceitam vírgula ou ponto decimal, como `19,90` e `19.90`, sem separador de milhar. Quantidades devem ser números inteiros. Preço e estoque iguais a zero são permitidos; a quantidade de uma compra deve ser maior que zero.

O checkout só é confirmado com `S` ou `sim`. Outra resposta cancela a operação e preserva carrinho e estoque. Fim da entrada encerra a aplicação sem travar.

## Exemplo de uso verificado

1. Cadastre **Livro**, preço **19,90**, estoque **5**.
2. Cadastre **Caneta**, preço **2,50**, estoque **10**.
3. Adicione **2 livros** e **3 canetas**.
4. Remova as canetas e adicione **1 caneta**.
5. Cancele o checkout uma vez e depois confirme.

Resultado observado ao executar o programa:

```text
Livro: 2 x R$ 19,90 = R$ 39,80
Caneta: 1 x R$ 2,50 = R$ 2,50
Total do pedido: R$ 42,30
Estoque restante: Livro = 3; Caneta = 9
Carrinho vazio após a confirmação
```

## Arquitetura

```text
MiniEcommerce.sln
MiniEcommerce.Console/
  Program.cs
  Apresentacao/
    LeitorDeEntrada.cs
  Dominio/
    Produto.cs
    CarrinhoDeCompras.cs
    Pedido.cs
  Servicos/
    Catalogo.cs
    ServicoDeCheckout.cs
MiniEcommerce.Tests/
  ProdutoTests.cs
  CatalogoTests.cs
  CarrinhoDeComprasTests.cs
  PedidoTests.cs
  ConsoleTests.cs
docs/
  TDD.md
  REQUISITOS.md
```

| Parte | Responsabilidade |
| --- | --- |
| `Program.cs` / `AplicacaoConsole` | Menu, comandos e apresentação dos resultados. |
| `LeitorDeEntrada` | Leitura e conversão de entradas do usuário. |
| `Produto` | Valores válidos de cadastro e controle da baixa de estoque. |
| `Catalogo` | Cadastro, geração de IDs e consulta dos produtos. |
| `CarrinhoDeCompras` | Quantidades acumuladas, limites de estoque, remoção e subtotal. |
| `ServicoDeCheckout` | Revalidação de todos os itens, criação do pedido e efetivação da compra. |
| `Pedido` / `ItemPedido` | Cópia dos dados financeiros da compra, independente do carrinho. |

Os namespaces separam `MiniEcommerce.Dominio`, `MiniEcommerce.Servicos` e `MiniEcommerce.ConsoleApp`. Os testes referenciam diretamente o projeto principal, conforme o enunciado.

Valores monetários usam `decimal`. O checkout valida todos os produtos e calcula o resumo **antes de alterar o estoque**, evitando alterações parciais por falta de estoque ou estouro do limite numérico. A aplicação é de uso sequencial no console; não implementa acesso concorrente por vários usuários.

## Testes e validação

```sh
dotnet test MiniEcommerce.sln
dotnet build MiniEcommerce.sln --configuration Release --warnaserror
dotnet test MiniEcommerce.sln --configuration Release
```

Validação final: **63 testes aprovados**, nenhum teste ignorado e compilação Release com **zero avisos e zero erros**.

| Área | Casos executados |
| --- | ---: |
| Produto | 12 |
| Catálogo | 5 |
| Carrinho | 16 |
| Pedido / checkout | 12 |
| Console | 18 |
| **Total** | **63** |

Os testes de console usam `StringReader` e `StringWriter` para executar o menu com entradas controladas, sem precisar digitar manualmente. Houve também uma execução real por `dotnet run`, com 11 verificações do fluxo de compra.

## TDD e histórico Git

Os testes de cada funcionalidade foram escritos e executados **antes da implementação**. O histórico preserva as etapas vermelhas e verdes:

- `chore:` estrutura inicial dos projetos.
- `test(produto):` e `feat(produto):` validações e catálogo.
- `test(carrinho):` e `feat(carrinho):` carrinho e estoque.
- `test(pedido):` e `feat(pedido):` checkout.
- `test(console):` e `feat(console):` interface.
- `refactor(core):` organização e legibilidade.
- `test(console):` e `fix(console):` caso limite de remoção com totais muito grandes.
- `docs:` documentação final.

Consulte [o registro do TDD](docs/TDD.md) para conferir os resultados reais de cada etapa e [o mapa de requisitos](docs/REQUISITOS.md) para localizar cada item do exercício.

Para consultar o histórico:

```sh
git log --oneline
git status
```

O repositório já possuía um commit automático de licença chamado `Initial commit`. Ele foi preservado; os commits da implementação seguem Conventional Commits.

## Licença

[MIT](LICENSE), preservada do repositório criado pelo proprietário.
