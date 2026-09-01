namespace RevisaoCodigoApp.Models;

public class MelhoriaCodigo : SolicitacaoMudanca
{
    /// <summary>
    /// Métrica ou aspecto de qualidade que a melhoria visa aprimorar.
    /// Exemplos: cobertura de testes, legibilidade, performance.
    /// </summary>
    public string MetricaAlvo { get; set; }

    public MelhoriaCodigo(string titulo, string descricao, Desenvolvedor autor, string metricaAlvo)
        : base(titulo, descricao, autor)
    {
        MetricaAlvo = metricaAlvo;
    }

    public override string Avaliar() =>
        $"Melhoria de Código — Título: \"{Titulo}\" | Métrica: {MetricaAlvo} | " +
        $"Autor: {Autor.Nome} | Status: {Status} | " +
        $"Etiquetas: [{string.Join(", ", Etiquetas)}]";
}
