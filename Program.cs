using LinqCrud;

var repo = new AlunoRepository();

if (!repo.GetAll().Any())
    Seed(repo);

while (true)
{
    Console.WriteLine("""

    ╔══════════════════════════════════╗
    ║   SISTEMA ESCOLAR — Alunos       ║
    ╠══════════════════════════════════╣
    ║ 1.  Listar todos os alunos       ║
    ║ 2.  Buscar por ID                ║
    ║ 3.  Filtrar por turma            ║
    ║ 4.  Filtrar por faixa de nota    ║
    ║ 5.  Listar aprovados             ║
    ║ 6.  Listar reprovados            ║
    ║ 7.  Agrupar por turma            ║
    ║ 8.  Pesquisar por nome/matrícula ║
    ║ 9.  Estatísticas                 ║
    ║ 10. Cadastrar aluno              ║
    ║ 11. Atualizar nota               ║
    ║ 12. Remover aluno                ║
    ║ 0.  Sair                         ║
    ╚══════════════════════════════════╝
    """);

    Console.Write("Opção: ");
    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            PrintAlunos(repo.GetAll(), "Todos os alunos");
            break;

        case "2":
            Console.Write("ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var a = repo.GetById(id);
                if (a is null) Console.WriteLine("Aluno não encontrado.");
                else PrintAluno(a);
            }
            break;

        case "3":
            Console.Write("Turma: ");
            var turma = Console.ReadLine() ?? "";
            PrintAlunos(repo.GetByTurma(turma), $"Turma: {turma}");
            break;

        case "4":
            Console.Write("Nota mínima: ");
            double.TryParse(Console.ReadLine(), out double min);
            Console.Write("Nota máxima: ");
            double.TryParse(Console.ReadLine(), out double max);
            PrintAlunos(repo.GetByFaixaNota(min, max), $"Notas de {min:F1} a {max:F1}");
            break;

        case "5":
            Console.Write("Média mínima de aprovação (padrão 5,0): ");
            var inputAp = Console.ReadLine();
            double mediaAp = string.IsNullOrWhiteSpace(inputAp) ? 5.0 : double.Parse(inputAp);
            PrintAlunos(repo.GetAprovados(mediaAp), $"Aprovados (nota ≥ {mediaAp:F1})");
            break;

        case "6":
            Console.Write("Média mínima de aprovação (padrão 5,0): ");
            var inputRep = Console.ReadLine();
            double mediaRep = string.IsNullOrWhiteSpace(inputRep) ? 5.0 : double.Parse(inputRep);
            PrintAlunos(repo.GetReprovados(mediaRep), $"Reprovados (nota < {mediaRep:F1})");
            break;

        case "7":
            Console.WriteLine("\n── Agrupado por Turma ──");
            foreach (var grupo in repo.GroupByTurma())
            {
                var mediaT = grupo.Average(a => a.Nota);
                Console.WriteLine($"\n  [{grupo.Key}]  {grupo.Count()} aluno(s)  |  média: {mediaT:F1}");
                foreach (var a in grupo)
                    Console.WriteLine($"    • {a.Nome,-25} {a.Matricula,-10}  nota: {a.Nota:F1}");
            }
            break;

        case "8":
            Console.Write("Buscar (nome ou matrícula): ");
            var termo = Console.ReadLine() ?? "";
            PrintAlunos(repo.Search(termo), $"Resultados para \"{termo}\"");
            break;

        case "9":
            var todos = repo.GetAll().ToList();
            Console.WriteLine($"\n── Estatísticas ──");
            Console.WriteLine($"  Total de alunos : {todos.Count}");
            Console.WriteLine($"  Média geral     : {repo.GetMediaGeral():F2}");
            Console.WriteLine($"  Aprovados       : {repo.GetAprovados().Count()}");
            Console.WriteLine($"  Reprovados      : {repo.GetReprovados().Count()}");
            var melhor = repo.GetMelhorAluno();
            if (melhor is not null)
                Console.WriteLine($"  Melhor nota     : {melhor.Nome} — {melhor.Nota:F1}");
            break;

        case "10":
            Console.Write("Nome          : "); var nome = Console.ReadLine() ?? "";
            Console.Write("Matrícula     : "); var matricula = Console.ReadLine() ?? "";
            Console.Write("Turma         : "); var turmaNew = Console.ReadLine() ?? "";
            Console.Write("Data nasc. (dd/MM/yyyy): ");
            DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", out DateOnly dataNasc);
            Console.Write("Nota          : ");
            double.TryParse(Console.ReadLine(), out double nota);

            var novo = repo.Add(new Aluno
            {
                Nome = nome,
                Matricula = matricula,
                Turma = turmaNew,
                DataNascimento = dataNasc,
                Nota = nota
            });
            Console.WriteLine($"Aluno cadastrado com ID {novo.Id}.");
            break;

        case "11":
            Console.Write("ID do aluno: ");
            if (int.TryParse(Console.ReadLine(), out int uid))
            {
                Console.Write("Nova nota: ");
                if (double.TryParse(Console.ReadLine(), out double novaNota))
                {
                    bool ok = repo.Update(uid, a => a.Nota = novaNota);
                    Console.WriteLine(ok ? "Nota atualizada." : "Aluno não encontrado.");
                }
            }
            break;

        case "12":
            Console.Write("ID do aluno: ");
            if (int.TryParse(Console.ReadLine(), out int did))
            {
                bool ok = repo.Delete(did);
                Console.WriteLine(ok ? "Aluno removido." : "Aluno não encontrado.");
            }
            break;

        case "0":
            Console.WriteLine("Até logo!");
            return;

        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}

// ── Helpers ───────────────────────────────────────────────────────────────────

static void PrintAlunos(IEnumerable<Aluno> alunos, string titulo)
{
    var lista = alunos.ToList();
    Console.WriteLine($"\n── {titulo} ({lista.Count} aluno(s)) ──");
    if (!lista.Any()) { Console.WriteLine("  (nenhum resultado)"); return; }

    Console.WriteLine($"  {"ID",-4} {"Nome",-25} {"Matrícula",-12} {"Turma",-8} {"Nota",6}");
    Console.WriteLine("  " + new string('─', 60));
    foreach (var a in lista)
        Console.WriteLine($"  {a.Id,-4} {a.Nome,-25} {a.Matricula,-12} {a.Turma,-8} {a.Nota,6:F1}");
}

static void PrintAluno(Aluno a)
{
    Console.WriteLine($"\n  ID            : {a.Id}");
    Console.WriteLine($"  Nome          : {a.Nome}");
    Console.WriteLine($"  Matrícula     : {a.Matricula}");
    Console.WriteLine($"  Turma         : {a.Turma}");
    Console.WriteLine($"  Data de nasc. : {a.DataNascimento:dd/MM/yyyy}");
    Console.WriteLine($"  Nota          : {a.Nota:F1}");
}

static void Seed(AlunoRepository repo)
{
    var alunos = new[]
    {
        new Aluno { Nome = "Ana Lima",        Matricula = "2024001", Turma = "3A", DataNascimento = new DateOnly(2007, 3, 15), Nota = 8.5 },
        new Aluno { Nome = "Bruno Souza",     Matricula = "2024002", Turma = "3A", DataNascimento = new DateOnly(2007, 7, 22), Nota = 4.0 },
        new Aluno { Nome = "Carla Mendes",    Matricula = "2024003", Turma = "2B", DataNascimento = new DateOnly(2008, 1, 10), Nota = 7.2 },
        new Aluno { Nome = "Diego Ferreira",  Matricula = "2024004", Turma = "2B", DataNascimento = new DateOnly(2008, 9, 5),  Nota = 3.8 },
        new Aluno { Nome = "Elisa Rocha",     Matricula = "2024005", Turma = "1C", DataNascimento = new DateOnly(2009, 5, 30), Nota = 9.1 },
        new Aluno { Nome = "Felipe Costa",    Matricula = "2024006", Turma = "1C", DataNascimento = new DateOnly(2009, 11, 18),Nota = 5.5 },
        new Aluno { Nome = "Gabriela Nunes",  Matricula = "2024007", Turma = "3A", DataNascimento = new DateOnly(2007, 2, 28), Nota = 6.8 },
        new Aluno { Nome = "Henrique Alves",  Matricula = "2024008", Turma = "2B", DataNascimento = new DateOnly(2008, 6, 14), Nota = 2.5 },
    };

    foreach (var a in alunos) repo.Add(a);
    Console.WriteLine("Base de dados criada com alunos de exemplo.\n");
}
