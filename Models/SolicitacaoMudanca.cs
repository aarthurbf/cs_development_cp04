using RevisaoCodigoApp.Enums;
using RevisaoCodigoApp.Interfaces;

namespace RevisaoCodigoApp.Models;

public abstract class SolicitacaoMudanca : IAvaliavel
{
    private static int _proximoId = 1;

    public int Id { get; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public Desenvolvedor Autor { get; }
    public HashSet<string> Etiquetas { get; } = new(StringComparer.OrdinalIgnoreCase);
    public StatusSolicitacao Status { get; set; } = StatusSolicitacao.Rascunho;
    public DateTime DataCriacao { get; } = DateTime.Now;

    protected SolicitacaoMudanca(string titulo, string descricao, Desenvolvedor autor)
    {
        Id = _proximoId++;
        Titulo = titulo;
        Descricao = descricao;
        Autor = autor;
    }

    /// <summary>
    /// Adiciona uma etiqueta ao HashSet (sem duplicatas, case-insensitive).
    /// </summary>
    public bool AdicionarEtiqueta(string etiqueta)
    {
        if (string.IsNullOrWhiteSpace(etiqueta)) return false;
        return Etiquetas.Add(etiqueta.Trim().ToLower());
    }

    /// <summary>
    /// Implementação polimórfica obrigatória em cada subclasse.
    /// </summary>
    public abstract string Avaliar();

    public override string ToString() =>
        $"[#{Id}] {Titulo} ({GetType().Name}) — Status: {Status}";
}
