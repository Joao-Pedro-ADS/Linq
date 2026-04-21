using LinqCrud.Models;

namespace LinqCrud.UI;

public static class AlunoView
{
    public static void PrintAlunos(IEnumerable<Aluno> alunos, string titulo)
    {
        var lista = alunos.ToList();
        Console.WriteLine($"\n── {titulo} ({lista.Count} aluno(s)) ──");
        if (!lista.Any()) { Console.WriteLine("  (nenhum resultado)"); return; }

        Console.WriteLine($"  {"ID",-4} {"Nome",-25} {"Matrícula",-12} {"Turma",-8} {"Nota",6}");
        Console.WriteLine("  " + new string('─', 60));
        foreach (var a in lista)
            Console.WriteLine($"  {a.Id,-4} {a.Nome,-25} {a.Matricula,-12} {a.Turma,-8} {a.Nota,6:F1}");
    }

    public static void PrintAluno(Aluno a)
    {
        Console.WriteLine($"\n  ID            : {a.Id}");
        Console.WriteLine($"  Nome          : {a.Nome}");
        Console.WriteLine($"  Matrícula     : {a.Matricula}");
        Console.WriteLine($"  Turma         : {a.Turma}");
        Console.WriteLine($"  Data de nasc. : {a.DataNascimento:dd/MM/yyyy}");
        Console.WriteLine($"  Nota          : {a.Nota:F1}");
    }
}
