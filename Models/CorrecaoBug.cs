namespace RevisaoCodigoApp.Models;

public class CorrecaoBug : SolicitacaoMudanca
{
    /// <summary>
    /// Nível de severidade do bug: Baixa, Media, Alta ou Critica.
    /// </summary>
    public string Severidade { get; set; }

    public CorrecaoBug(string titulo, string descricao, Desenvolvedor autor, string severidade)
        : base(titulo, descricao, autor)
    {
        Severidade = severidade;
    }

    public override string Avaliar() =>
        $"Correção de Bug — Título: \"{Titulo}\" | Severidade: {Severidade} | " +
        $"Autor: {Autor.Nome} | Status: {Status} | " +
        $"Etiquetas: [{string.Join(", ", Etiquetas)}]";
}
