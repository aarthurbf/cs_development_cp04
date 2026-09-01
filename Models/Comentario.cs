namespace RevisaoCodigoApp.Models;

public class Comentario
{
    private static int _proximoId = 1;

    public int Id { get; }
    public string Texto { get; }
    public Desenvolvedor Autor { get; }
    public DateTime DataHora { get; } = DateTime.Now;

    public Comentario(string texto, Desenvolvedor autor)
    {
        Id = _proximoId++;
        Texto = texto;
        Autor = autor;
    }

    public override string ToString() =>
        $"  [{DataHora:dd/MM/yyyy HH:mm}] {Autor.Nome}: {Texto}";
}
