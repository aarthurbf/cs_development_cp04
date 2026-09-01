using RevisaoCodigoApp.Collections;
using RevisaoCodigoApp.Enums;
using RevisaoCodigoApp.Models;

namespace RevisaoCodigoApp;

class Program
{
    // ─── Estado global em memória ────────────────────────────────────────────
    static readonly PainelRevisoes<SolicitacaoMudanca> Painel = new();

    // Referência às revisões em andamento para facilitar operações por índice
    // (a List<Revisao> já vive dentro de Painel.RevisoesRealizadas)

    // ────────────────────────────────────────────────────────────────────────
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CarregarDadosDemonstracao();
        ExibirBanner();

        bool continuar = true;
        while (continuar)
        {
            ExibirMenu();
            string opcao = LerLinha("Opção");
            Console.WriteLine();
            continuar = ProcessarOpcao(opcao);
        }

        Console.WriteLine("\nEncerrando o sistema. Até logo!");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Banner e Menu
    // ────────────────────────────────────────────────────────────────────────

    static void ExibirBanner()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║      SISTEMA DE REVISÃO DE CÓDIGO — CS/POO 2024      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();
    }

    static void ExibirMenu()
    {
        Console.WriteLine("┌─────────────────────────────────────────────────────┐");
        Console.WriteLine("│                      MENU PRINCIPAL                  │");
        Console.WriteLine("├─────────────────────────────────────────────────────┤");
        Console.WriteLine("│  1. Cadastrar desenvolvedor                           │");
        Console.WriteLine("│  2. Criar solicitação de mudança                      │");
        Console.WriteLine("│  3. Adicionar etiquetas a uma solicitação             │");
        Console.WriteLine("│  4. Enviar solicitação para fila de revisão           │");
        Console.WriteLine("│  5. Atribuir próxima solicitação da fila a revisor    │");
        Console.WriteLine("│  6. Adicionar comentário a uma revisão                │");
        Console.WriteLine("│  7. Registrar decisão de revisão                      │");
        Console.WriteLine("│  8. Exibir histórico de decisões                      │");
        Console.WriteLine("│  9. Exibir estatísticas dos revisores                 │");
        Console.WriteLine("│ 10. Listar solicitações                               │");
        Console.WriteLine("│ 11. Listar revisões                                   │");
        Console.WriteLine("│  0. Sair                                              │");
        Console.WriteLine("└─────────────────────────────────────────────────────┘");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Dispatcher de opções
    // ────────────────────────────────────────────────────────────────────────

    static bool ProcessarOpcao(string opcao)
    {
        switch (opcao.Trim())
        {
            case "1":  CadastrarDesenvolvedor();          break;
            case "2":  CriarSolicitacao();                break;
            case "3":  AdicionarEtiquetas();              break;
            case "4":  EnviarParaFila();                  break;
            case "5":  AtribuirProxima();                 break;
            case "6":  AdicionarComentario();             break;
            case "7":  RegistrarDecisao();                break;
            case "8":  ExibirHistorico();                 break;
            case "9":  ExibirEstatisticas();              break;
            case "10": ListarSolicitacoes();              break;
            case "11": ListarRevisoes();                  break;
            case "0":  return false;
            default:
                Erro("Opção inválida. Digite um número de 0 a 11.");
                break;
        }
        Console.WriteLine();
        Pausar();
        Console.Clear();
        ExibirBanner();
        return true;
    }

    // ────────────────────────────────────────────────────────────────────────
    //  1 — Cadastrar desenvolvedor
    // ────────────────────────────────────────────────────────────────────────

    static void CadastrarDesenvolvedor()
    {
        Titulo("CADASTRAR DESENVOLVEDOR");
        string nome         = LerLinha("Nome");
        string especialidade = LerLinha("Especialidade (ex: Backend, Frontend, DevOps)");

        var dev = new Desenvolvedor(nome, especialidade);
        Painel.CadastrarDesenvolvedor(dev);
        Ok($"Desenvolvedor cadastrado com sucesso! {dev}");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  2 — Criar solicitação de mudança
    // ────────────────────────────────────────────────────────────────────────

    static void CriarSolicitacao()
    {
        Titulo("CRIAR SOLICITAÇÃO DE MUDANÇA");

        if (Painel.Desenvolvedores.Count == 0)
        {
            Erro("Nenhum desenvolvedor cadastrado. Cadastre um desenvolvedor primeiro (opção 1).");
            return;
        }

        Console.WriteLine("Tipo de solicitação:");
        Console.WriteLine("  1. Correção de Bug");
        Console.WriteLine("  2. Nova Funcionalidade");
        Console.WriteLine("  3. Melhoria de Código");
        string tipo = LerLinha("Tipo");

        if (tipo is not ("1" or "2" or "3"))
        {
            Erro("Tipo inválido. Escolha 1, 2 ou 3.");
            return;
        }

        string titulo    = LerLinha("Título");
        string descricao = LerLinha("Descrição");

        ListarDesenvolvedores();
        if (!int.TryParse(LerLinha("Id do autor"), out int autorId) ||
            !Painel.Desenvolvedores.TryGetValue(autorId, out Desenvolvedor? autor))
        {
            Erro("Desenvolvedor não encontrado.");
            return;
        }

        SolicitacaoMudanca solicitacao = tipo switch
        {
            "1" => new CorrecaoBug(titulo, descricao, autor,
                       LerLinha("Severidade (Baixa / Media / Alta / Critica)")),
            "2" => new NovaFuncionalidade(titulo, descricao, autor,
                       LerLinha("Módulo alvo")),
            "3" => new MelhoriaCodigo(titulo, descricao, autor,
                       LerLinha("Métrica alvo (ex: cobertura de testes, legibilidade)")),
            _   => throw new InvalidOperationException()
        };

        Painel.RegistrarSolicitacao(solicitacao);
        Ok($"Solicitação criada! {solicitacao}");
        Console.WriteLine($"\nAvaliação automática:\n  {solicitacao.Avaliar()}");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  3 — Adicionar etiquetas
    // ────────────────────────────────────────────────────────────────────────

    static void AdicionarEtiquetas()
    {
        Titulo("ADICIONAR ETIQUETAS");

        var sol = SelecionarSolicitacao();
        if (sol is null) return;

        Console.WriteLine("Sugestões: bug, melhoria, documentacao, refatoracao, segurança, performance");
        string entrada = LerLinha("Etiquetas (separadas por vírgula)");

        int adicionadas = 0;
        foreach (string tag in entrada.Split(','))
        {
            if (sol.AdicionarEtiqueta(tag.Trim()))
                adicionadas++;
        }

        Ok($"{adicionadas} etiqueta(s) adicionada(s). Etiquetas atuais: [{string.Join(", ", sol.Etiquetas)}]");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  4 — Enviar para fila de revisão
    // ────────────────────────────────────────────────────────────────────────

    static void EnviarParaFila()
    {
        Titulo("ENVIAR PARA FILA DE REVISÃO");

        // Mostra apenas solicitações em Rascunho (ainda não enviadas)
        var candidatas = Painel.TodasSolicitacoes
            .Where(s => s.Status == StatusSolicitacao.Rascunho)
            .ToList();

        if (candidatas.Count == 0)
        {
            Erro("Nenhuma solicitação em rascunho disponível para envio.");
            return;
        }

        Console.WriteLine("Solicitações em rascunho:");
        foreach (var s in candidatas)
            Console.WriteLine($"  {s}");

        if (!int.TryParse(LerLinha("Id da solicitação"), out int id))
        {
            Erro("Id inválido.");
            return;
        }

        var sol = candidatas.Find(s => s.Id == id);
        if (sol is null)
        {
            Erro("Solicitação não encontrada ou não está em rascunho.");
            return;
        }

        Painel.EnqueueSolicitacao(sol);
        Ok($"Solicitação enviada para fila! Fila atual: {Painel.FilaEspera.Count} item(s).");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  5 — Atribuir próxima da fila a um revisor
    // ────────────────────────────────────────────────────────────────────────

    static void AtribuirProxima()
    {
        Titulo("ATRIBUIR PRÓXIMA SOLICITAÇÃO");

        if (Painel.FilaEspera.Count == 0)
        {
            Erro("A fila de revisão está vazia.");
            return;
        }

        Console.WriteLine($"Próxima na fila: \"{Painel.FilaEspera.Peek().Titulo}\"");
        Console.WriteLine();

        ListarDesenvolvedores();
        if (!int.TryParse(LerLinha("Id do revisor"), out int revisorId))
        {
            Erro("Id inválido.");
            return;
        }

        Revisao? revisao = Painel.AtribuirProxima(revisorId);
        if (revisao is null)
        {
            Erro("Revisor não encontrado ou fila vazia.");
            return;
        }

        Ok($"Revisão criada! {revisao}");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  6 — Adicionar comentário à revisão
    // ────────────────────────────────────────────────────────────────────────

    static void AdicionarComentario()
    {
        Titulo("ADICIONAR COMENTÁRIO À REVISÃO");

        var revisao = SelecionarRevisaoAtiva();
        if (revisao is null) return;

        ListarDesenvolvedores();
        if (!int.TryParse(LerLinha("Id do autor do comentário"), out int autorId) ||
            !Painel.Desenvolvedores.TryGetValue(autorId, out Desenvolvedor? autor))
        {
            Erro("Desenvolvedor não encontrado.");
            return;
        }

        string texto = LerLinha("Comentário técnico");
        var comentario = new Comentario(texto, autor);
        revisao.AdicionarComentario(comentario);

        Ok($"Comentário adicionado à revisão #{revisao.Id}. Total: {revisao.Comentarios.Count} comentário(s).");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  7 — Registrar decisão de revisão
    // ────────────────────────────────────────────────────────────────────────

    static void RegistrarDecisao()
    {
        Titulo("REGISTRAR DECISÃO DE REVISÃO");

        var revisao = SelecionarRevisaoAtiva();
        if (revisao is null) return;

        Console.WriteLine("Decisão:");
        Console.WriteLine("  1. Aprovar");
        Console.WriteLine("  2. Solicitar Ajustes");
        Console.WriteLine("  3. Reprovar");
        string escolha = LerLinha("Decisão");

        DecisaoRevisao decisao = escolha switch
        {
            "1" => DecisaoRevisao.Aprovado,
            "2" => DecisaoRevisao.AjustesSolicitados,
            "3" => DecisaoRevisao.Reprovado,
            _   => (DecisaoRevisao)(-1)
        };

        if ((int)decisao == -1)
        {
            Erro("Opção inválida. Escolha 1, 2 ou 3.");
            return;
        }

        revisao.RegistrarDecisao(decisao);

        string entrada =
            $"[{DateTime.Now:dd/MM/yyyy HH:mm}] Decisão: {decisao} | " +
            $"Solicitação: \"{revisao.Solicitacao.Titulo}\" | " +
            $"Revisor: {revisao.Revisor.Nome}";
        Painel.RegistrarDecisaoHistorico(entrada);

        Ok($"Decisão registrada! Novo status da solicitação: {revisao.Solicitacao.Status}");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  8 — Exibir histórico de decisões (Stack — ordem reversa)
    // ────────────────────────────────────────────────────────────────────────

    static void ExibirHistorico()
    {
        Titulo("HISTÓRICO DE DECISÕES (mais recente primeiro)");

        if (Painel.HistoricoDecisoes.Count == 0)
        {
            Console.WriteLine("  Nenhuma decisão registrada ainda.");
            return;
        }

        int i = 1;
        foreach (string entrada in Painel.HistoricoDecisoes)
            Console.WriteLine($"  {i++}. {entrada}");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  9 — Estatísticas de revisores
    // ────────────────────────────────────────────────────────────────────────

    static void ExibirEstatisticas()
    {
        Titulo("ESTATÍSTICAS DOS REVISORES");

        var stats = Painel.ObterEstatisticasRevisores();

        if (stats.Count == 0)
        {
            Console.WriteLine("  Nenhum desenvolvedor cadastrado.");
            return;
        }

        Console.WriteLine($"  {"Desenvolvedor",-25} {"Revisões",10}");
        Console.WriteLine($"  {"─────────────────────────",25} {"──────────",10}");
        foreach (var (nome, total) in stats.OrderByDescending(kv => kv.Value))
            Console.WriteLine($"  {nome,-25} {total,10}");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  10 — Listar solicitações
    // ────────────────────────────────────────────────────────────────────────

    static void ListarSolicitacoes()
    {
        Titulo("SOLICITAÇÕES DE MUDANÇA");

        if (Painel.TodasSolicitacoes.Count == 0)
        {
            Console.WriteLine("  Nenhuma solicitação criada.");
            return;
        }

        foreach (var sol in Painel.TodasSolicitacoes)
        {
            Console.WriteLine($"\n  {sol}");
            Console.WriteLine($"  Avaliação: {sol.Avaliar()}");
            if (sol.Etiquetas.Count > 0)
                Console.WriteLine($"  Etiquetas:  [{string.Join(", ", sol.Etiquetas)}]");
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  11 — Listar revisões
    // ────────────────────────────────────────────────────────────────────────

    static void ListarRevisoes()
    {
        Titulo("REVISÕES REALIZADAS");

        if (Painel.RevisoesRealizadas.Count == 0)
        {
            Console.WriteLine("  Nenhuma revisão realizada.");
            return;
        }

        foreach (var rev in Painel.RevisoesRealizadas)
        {
            Console.WriteLine($"\n  {rev}");
            if (rev.Comentarios.Count > 0)
            {
                Console.WriteLine("  Comentários:");
                foreach (var c in rev.Comentarios)
                    Console.WriteLine($"    {c}");
            }
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Helpers de seleção
    // ────────────────────────────────────────────────────────────────────────

    static SolicitacaoMudanca? SelecionarSolicitacao()
    {
        if (Painel.TodasSolicitacoes.Count == 0)
        {
            Erro("Nenhuma solicitação cadastrada.");
            return null;
        }

        Console.WriteLine("Solicitações disponíveis:");
        foreach (var s in Painel.TodasSolicitacoes)
            Console.WriteLine($"  {s}");

        if (!int.TryParse(LerLinha("Id da solicitação"), out int id))
        {
            Erro("Id inválido.");
            return null;
        }

        var sol = Painel.TodasSolicitacoes.FirstOrDefault(s => s.Id == id);
        if (sol is null) Erro("Solicitação não encontrada.");
        return sol;
    }

    static Revisao? SelecionarRevisaoAtiva()
    {
        var ativas = Painel.RevisoesRealizadas.Where(r => r.Decisao == null).ToList();

        if (ativas.Count == 0)
        {
            Erro("Nenhuma revisão em andamento.");
            return null;
        }

        Console.WriteLine("Revisões em andamento:");
        foreach (var r in ativas)
            Console.WriteLine($"  {r}");

        if (!int.TryParse(LerLinha("Id da revisão"), out int id))
        {
            Erro("Id inválido.");
            return null;
        }

        var revisao = ativas.Find(r => r.Id == id);
        if (revisao is null) Erro("Revisão não encontrada ou já finalizada.");
        return revisao;
    }

    static void ListarDesenvolvedores()
    {
        Console.WriteLine("Desenvolvedores cadastrados:");
        foreach (var dev in Painel.Desenvolvedores.Values)
            Console.WriteLine($"  {dev}");
        Console.WriteLine();
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Dados de demonstração pré-carregados
    // ────────────────────────────────────────────────────────────────────────

    static void CarregarDadosDemonstracao()
    {
        // Desenvolvedores
        var ana   = new Desenvolvedor("Ana Souza",    "Backend");
        var pedro = new Desenvolvedor("Pedro Lima",   "Frontend");
        var julia = new Desenvolvedor("Julia Mendes", "DevOps");

        Painel.CadastrarDesenvolvedor(ana);
        Painel.CadastrarDesenvolvedor(pedro);
        Painel.CadastrarDesenvolvedor(julia);

        // Solicitações
        var bug = new CorrecaoBug(
            "Falha no cálculo de desconto",
            "Desconto aplicado incorretamente para clientes premium",
            ana, "Alta");
        bug.AdicionarEtiqueta("bug");
        bug.AdicionarEtiqueta("backend");

        var feat = new NovaFuncionalidade(
            "Exportar relatório em PDF",
            "Permitir que usuários exportem relatórios mensais em formato PDF",
            pedro, "Relatórios");
        feat.AdicionarEtiqueta("melhoria");
        feat.AdicionarEtiqueta("documentacao");

        var refactor = new MelhoriaCodigo(
            "Refatorar módulo de autenticação",
            "Substituir lógica legada por padrão OAuth2",
            julia, "cobertura de testes");
        refactor.AdicionarEtiqueta("refatoracao");
        refactor.AdicionarEtiqueta("segurança");

        Painel.RegistrarSolicitacao(bug);
        Painel.RegistrarSolicitacao(feat);
        Painel.RegistrarSolicitacao(refactor);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Utilitários de I/O
    // ────────────────────────────────────────────────────────────────────────

    static string LerLinha(string prompt)
    {
        Console.Write($"  {prompt}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    static void Ok(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  ✔ {msg}");
        Console.ResetColor();
    }

    static void Erro(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ✖ {msg}");
        Console.ResetColor();
    }

    static void Titulo(string titulo)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"── {titulo} ──");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void Pausar()
    {
        Console.WriteLine("\n  Pressione qualquer tecla para continuar...");
        Console.ReadKey(true);
    }
}
