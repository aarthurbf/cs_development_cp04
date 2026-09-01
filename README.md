# Sistema de Revisão de Código — CS/POO 2024

## Integrantes

- Arthur Bobadilla Franchi | RM555056
- Luan Orlandelli Ramos | RM554747
- Jorge Luis | RM554418

---

## Como executar

### Pré-requisitos

- [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) (ou superior)

### Passos

```bash
# 1. Entrar na pasta do projeto
cd RevisaoCodigoApp

# 2. Compilar
dotnet build

# 3. Executar
dotnet run
```

O sistema inicia com **3 desenvolvedores e 3 solicitações** pré-carregadas para facilitar os testes
sem precisar criar dados do zero.

---

## Fluxo básico de uso

1. Use a opção **4** para enviar uma solicitação para a fila de revisão.
2. Use a opção **5** para atribuir a próxima solicitação a um revisor.
3. Use a opção **6** para adicionar comentários técnicos à revisão aberta.
4. Use a opção **7** para registrar a decisão final (Aprovar / Solicitar Ajustes / Reprovar).
5. Use as opções **8** e **9** para consultar o histórico e as estatísticas.

---

## Modelagem adotada

### Hierarquia de classes

```
SolicitacaoMudanca   (classe abstrata — implementa IAvaliavel)
├── CorrecaoBug          → atributo extra: Severidade
├── NovaFuncionalidade   → atributo extra: ModuloAlvo
└── MelhoriaCodigo       → atributo extra: MetricaAlvo
```

Cada subclasse sobrescreve `Avaliar()`, permitindo tratamento **polimórfico** em tempo de execução.

### Classes de suporte

| Classe | Responsabilidade |
|---|---|
| `Desenvolvedor` | Representa um membro da equipe; mantém contador de revisões atribuídas |
| `Comentario` | Texto técnico com autor e timestamp |
| `Revisao` | Vincula solicitação + revisor; gerencia `List<Comentario>` e decisão final |

### Classe genérica

`PainelRevisoes<T> where T : SolicitacaoMudanca` centraliza todas as estruturas de dados:

| Estrutura | Campo | Papel no sistema |
|---|---|---|
| `Queue<T>` | `FilaEspera` | Ordem FIFO das solicitações aguardando revisão |
| `Stack<string>` | `HistoricoDecisoes` | Registro de decisões em ordem reversa (mais recente primeiro) |
| `Dictionary<int, Desenvolvedor>` | `Desenvolvedores` | Acesso rápido por Id |
| `List<Revisao>` | `RevisoesRealizadas` | Histórico completo de revisões |
| `HashSet<string>` | `SolicitacaoMudanca.Etiquetas` | Tags sem duplicatas (case-insensitive) |

### Interface

`IAvaliavel` (namespace `Interfaces`) declara `string Avaliar()`.  
`SolicitacaoMudanca` implementa a interface; cada subclasse define a mensagem de avaliação.

---

## Estrutura de arquivos

```
RevisaoCodigoApp/
├── RevisaoCodigoApp.csproj
├── Program.cs                  ← ponto de entrada + menu interativo
├── Models/
│   ├── Desenvolvedor.cs
│   ├── SolicitacaoMudanca.cs   ← classe abstrata + IAvaliavel
│   ├── CorrecaoBug.cs
│   ├── NovaFuncionalidade.cs
│   ├── MelhoriaCodigo.cs
│   ├── Revisao.cs
│   └── Comentario.cs
├── Interfaces/
│   └── IAvaliavel.cs
├── Enums/
│   ├── StatusSolicitacao.cs
│   └── DecisaoRevisao.cs
└── Collections/
    └── PainelRevisoes.cs       ← PainelRevisoes<T> (classe genérica)
```
