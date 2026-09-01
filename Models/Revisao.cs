using RevisaoCodigoApp.Enums;

namespace RevisaoCodigoApp.Models;

public class Revisao
{
    private static int _proximoId = 1;

    public int Id { get; }
    public SolicitacaoMudanca Solicitacao { get; }
    public Desenvolvedor Revisor { get; }

    /// <summary>
    /// Lista de comentários técnicos desta revisão — armazenada em List&lt;T&gt;.
    /// </summary>
    public List<Comentario> Comentarios { get; } = new();

    public DateTime DataAtribuicao { get; } = DateTime.Now;

    /// <summary>
    /// Decisão registrada ao finalizar a revisão. Null enquanto em andamento.
    /// </summary>
    public DecisaoRevisao? Decisao { get; private set; }

    public Revisao(SolicitacaoMudanca solicitacao, Desenvolvedor revisor)
    {
        Id = _proximoId++;
        Solicitacao = solicitacao;
        Revisor = revisor;
    }

    /// <summary>
    /// Adiciona um comentário técnico à revisão.
    /// </summary>
    public void AdicionarComentario(Comentario comentario)
    {
        Comentarios.Add(comentario);
    }

    /// <summary>
    /// Registra a decisão final e atualiza o status da solicitação vinculada.
    /// </summary>
    public void RegistrarDecisao(DecisaoRevisao decisao)
    {
        Decisao = decisao;
        Solicitacao.Status = decisao switch
        {
            DecisaoRevisao.Aprovado          => StatusSolicitacao.Aprovado,
            DecisaoRevisao.AjustesSolicitados => StatusSolicitacao.AjustesSolicitados,
            DecisaoRevisao.Reprovado         => StatusSolicitacao.Reprovado,
            _                                => Solicitacao.Status
        };
    }

    public override string ToString() =>
        $"[Revisão #{Id}] Solicitação: {Solicitacao.Titulo} | Revisor: {Revisor.Nome} | " +
        $"Decisão: {(Decisao.HasValue ? Decisao.Value.ToString() : "Em andamento")} | " +
        $"Comentários: {Comentarios.Count}";
}
