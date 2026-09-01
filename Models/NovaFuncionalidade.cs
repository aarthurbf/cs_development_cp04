namespace RevisaoCodigoApp.Models;

public class NovaFuncionalidade : SolicitacaoMudanca
{
    /// <summary>
    /// Módulo do sistema onde a nova funcionalidade será implementada.
    /// </summary>
    public string ModuloAlvo { get; set; }

    public NovaFuncionalidade(string titulo, string descricao, Desenvolvedor autor, string moduloAlvo)
        : base(titulo, descricao, autor)
    {
        ModuloAlvo = moduloAlvo;
    }

    public override string Avaliar() =>
        $"Nova Funcionalidade — Título: \"{Titulo}\" | Módulo: {ModuloAlvo} | " +
        $"Autor: {Autor.Nome} | Status: {Status} | " +
        $"Etiquetas: [{string.Join(", ", Etiquetas)}]";
}
