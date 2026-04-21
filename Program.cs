using LinqCrud.Data;
using LinqCrud.Models;
using LinqCrud.Repository;
using LinqCrud.UI;

var repo = new AlunoRepository();

if (!repo.GetAll().Any())
    Seed.Populate(repo);

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
            AlunoView.PrintAlunos(repo.GetAll(), "Todos os alunos");
            break;

        case "2":
            Console.Write("ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var a = repo.GetById(id);
                if (a is null) Console.WriteLine("Aluno não encontrado.");
                else AlunoView.PrintAluno(a);
            }
            break;

        case "3":
            Console.Write("Turma: ");
            var turma = Console.ReadLine() ?? "";
            AlunoView.PrintAlunos(repo.GetByTurma(turma), $"Turma: {turma}");
            break;

        case "4":
            Console.Write("Nota mínima: ");
            double.TryParse(Console.ReadLine(), out double min);
            Console.Write("Nota máxima: ");
            double.TryParse(Console.ReadLine(), out double max);
            AlunoView.PrintAlunos(repo.GetByFaixaNota(min, max), $"Notas de {min:F1} a {max:F1}");
            break;

        case "5":
            Console.Write("Média mínima de aprovação (padrão 5,0): ");
            var inputAp = Console.ReadLine();
            double mediaAp = string.IsNullOrWhiteSpace(inputAp) ? 5.0 : double.Parse(inputAp);
            AlunoView.PrintAlunos(repo.GetAprovados(mediaAp), $"Aprovados (nota ≥ {mediaAp:F1})");
            break;

        case "6":
            Console.Write("Média mínima de aprovação (padrão 5,0): ");
            var inputRep = Console.ReadLine();
            double mediaRep = string.IsNullOrWhiteSpace(inputRep) ? 5.0 : double.Parse(inputRep);
            AlunoView.PrintAlunos(repo.GetReprovados(mediaRep), $"Reprovados (nota < {mediaRep:F1})");
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
            AlunoView.PrintAlunos(repo.Search(termo), $"Resultados para \"{termo}\"");
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
