# Registro de TDD

Cada linha abaixo corresponde a uma execução real de `dotnet test`, com relatório TRX conferido.

| Execução | Fase esperada | Total | Aprovados | Falhas |
| --- | --- | ---: | ---: | ---: |
| produto-red | Vermelho | 17 | 0 | 17 |
| produto-green | Verde | 17 | 17 | 0 |
| carrinho-red | Vermelho | 33 | 17 | 16 |
| carrinho-green | Verde | 33 | 33 | 0 |
| pedido-red | Vermelho | 45 | 33 | 12 |
| pedido-green | Verde | 45 | 45 | 0 |
| console-red | Vermelho | 62 | 45 | 17 |
| console-green | Verde | 62 | 62 | 0 |
| refactor-green | Verde | 62 | 62 | 0 |
| recuperacao-red | Vermelho | 63 | 62 | 1 |
| recuperacao-green | Verde | 63 | 63 | 0 |
| release | Verde | 63 | 63 | 0 |

## Como ler as fases

- **Vermelho:** os novos testes falham porque a funcionalidade ainda não foi implementada. Os testes anteriores continuam aprovados.
- **Verde:** a implementação faz a suíte completa passar.
- Os commits `test(...)` incluem somente os testes e os contratos mínimos das classes necessários para compilar. Os métodos ainda não implementados lançam `NotImplementedException`; as implementações aparecem nos commits `feat(...)` seguintes.
- A refatoração foi validada sem introduzir mudança de comportamento.

## Caso limite encontrado

Uma quantidade válida de um produto com preço muito alto pode exceder o limite de `decimal` ao calcular o subtotal. O checkout já falhava antes de alterar o estoque. Foi acrescentado um teste mostrando que a opção de remover itens também precisava funcionar nesse cenário. A correção passou a listar identificadores e quantidades sem depender do cálculo financeiro para permitir a remoção.

## Validação final

- Compilação Release: 0 avisos, 0 erros.
- Testes Release: 63 aprovados, 0 falhas, 0 ignorados.
- Execução real por `dotnet run`: cadastro, catálogo, adição, consulta, remoção, cancelamento e confirmação de compra; 11 verificações aprovadas.
- Exemplo verificado: total de R$ 42,30, estoques restantes 3 e 9, carrinho vazio.

Os relatórios temporários e diretórios `bin/obj` não são versionados. Este registro contém os resultados relevantes, sem caminhos locais da máquina.
