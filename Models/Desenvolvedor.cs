namespace RevisaoCodigoApp.Models;

public class Desenvolvedor
{
    private static int _proximoId = 1;

    public int Id { get; }
    public string Nome { get; set; }
    public string Especialidade { get; set; }

    /// <summary>
    /// Contador de revisões atribuídas a este desenvolvedor.
    /// Incrementado automaticamente por PainelRevisoes ao atribuir uma revisão.
    /// </summary>
    public int TotalRevisoes { get; private set; }

    public Desenvolvedor(string nome, string especialidade)
    {
        Id = _proximoId++;
        Nome = nome;
        Especialidade = especialidade;
    }

    public void IncrementarRevisoes() => TotalRevisoes++;

    public override string ToString() =>
        $"[#{Id}] {Nome} | Especialidade: {Especialidade} | Revisões: {TotalRevisoes}";
}
