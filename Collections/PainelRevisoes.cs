using RevisaoCodigoApp.Enums;
using RevisaoCodigoApp.Models;

namespace RevisaoCodigoApp.Collections;

public class PainelRevisoes<T> where T : SolicitacaoMudanca
{
    //  Estruturas de dados
    public Queue<T> FilaEspera { get; } = new();
    public Stack<string> HistoricoDecisoes { get; } = new();
    public Dictionary<int, Desenvolvedor> Desenvolvedores { get; } = new();
    public List<Revisao> RevisoesRealizadas { get; } = new();

    //  Catálogo de solicitações (todas, independente do status) 
    private readonly List<T> _todasSolicitacoes = new();
    public IReadOnlyList<T> TodasSolicitacoes => _todasSolicitacoes.AsReadOnly();

    //  Desenvolvedores

    /// <summary>
    /// Registra um desenvolvedor no painel (Dictionary por Id).
    /// </summary>
    public void CadastrarDesenvolvedor(Desenvolvedor dev)
    {
        if (Desenvolvedores.ContainsKey(dev.Id))
            throw new InvalidOperationException($"Desenvolvedor com Id {dev.Id} já cadastrado.");
        Desenvolvedores[dev.Id] = dev;
    }

    //  Solicitações

    /// <summary>
    /// Registra uma solicitação no catálogo geral sem enviá-la para a fila.
    /// </summary>
    public void RegistrarSolicitacao(T solicitacao)
    {
        _todasSolicitacoes.Add(solicitacao);
    }

    /// <summary>
    /// Envia uma solicitação para a fila de revisão (Queue) e atualiza seu status.
    /// </summary>
    public void EnqueueSolicitacao(T solicitacao)
    {
        if (!_todasSolicitacoes.Contains(solicitacao))
            _todasSolicitacoes.Add(solicitacao);

        solicitacao.Status = StatusSolicitacao.AguardandoRevisao;
        FilaEspera.Enqueue(solicitacao);
    }

    //  Revisões

    /// <summary>
    /// Retira a próxima solicitação da fila (Dequeue), cria uma Revisao,
    /// atribui ao revisor e incrementa seu contador de revisões.
    /// Retorna null se a fila estiver vazia ou o revisor não existir.
    /// </summary>
    public Revisao? AtribuirProxima(int revisorId)
    {
        if (!Desenvolvedores.TryGetValue(revisorId, out Desenvolvedor? revisor))
            return null;

        if (FilaEspera.Count == 0)
            return null;

        T solicitacao = FilaEspera.Dequeue();
        solicitacao.Status = StatusSolicitacao.EmRevisao;

        var revisao = new Revisao(solicitacao, revisor);
        revisor.IncrementarRevisoes();
        RevisoesRealizadas.Add(revisao);

        RegistrarDecisaoHistorico(
            $"[{DateTime.Now:dd/MM/yyyy HH:mm}] Solicitação \"{solicitacao.Titulo}\" " +
            $"atribuída a {revisor.Nome}");

        return revisao;
    }

    /// <summary>
    /// Registra uma entrada no histórico de decisões (Stack — ordem reversa).
    /// </summary>
    public void RegistrarDecisaoHistorico(string entrada)
    {
        HistoricoDecisoes.Push(entrada);
    }

    //  Consultas

    /// <summary>
    /// Retorna um dicionário nome → total de revisões para exibição de estatísticas.
    /// </summary>
    public Dictionary<string, int> ObterEstatisticasRevisores()
    {
        var stats = new Dictionary<string, int>();
        foreach (var dev in Desenvolvedores.Values)
            stats[dev.Nome] = dev.TotalRevisoes;
        return stats;
    }

    /// <summary>
    /// Busca uma revisão ativa (sem decisão) pela solicitação, se existir.
    /// </summary>
    public Revisao? ObterRevisaoAtiva(int solicitacaoId) =>
        RevisoesRealizadas.Find(r => r.Solicitacao.Id == solicitacaoId && r.Decisao == null);

    /// <summary>
    /// Busca qualquer revisão pelo Id da revisão.
    /// </summary>
    public Revisao? ObterRevisaoPorId(int revisaoId) =>
        RevisoesRealizadas.Find(r => r.Id == revisaoId);
}
