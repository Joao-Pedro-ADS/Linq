using Linq.Models;
using Linq.Repository;

namespace Linq.Data;

public static class Seed
{
    public static void Populate(AlunoRepository repo)
    {
        var alunos = new[]
        {
            new Aluno { Nome = "Ana Lima",        Matricula = "2024001", Turma = "3A", DataNascimento = new DateOnly(2007, 3, 15),  Nota = 8.5 },
            new Aluno { Nome = "Bruno Souza",     Matricula = "2024002", Turma = "3A", DataNascimento = new DateOnly(2007, 7, 22),  Nota = 4.0 },
            new Aluno { Nome = "Carla Mendes",    Matricula = "2024003", Turma = "2B", DataNascimento = new DateOnly(2008, 1, 10),  Nota = 7.2 },
            new Aluno { Nome = "Diego Ferreira",  Matricula = "2024004", Turma = "2B", DataNascimento = new DateOnly(2008, 9, 5),   Nota = 3.8 },
            new Aluno { Nome = "Elisa Rocha",     Matricula = "2024005", Turma = "1C", DataNascimento = new DateOnly(2009, 5, 30),  Nota = 9.1 },
            new Aluno { Nome = "Felipe Costa",    Matricula = "2024006", Turma = "1C", DataNascimento = new DateOnly(2009, 11, 18), Nota = 5.5 },
            new Aluno { Nome = "Gabriela Nunes",  Matricula = "2024007", Turma = "3A", DataNascimento = new DateOnly(2007, 2, 28),  Nota = 6.8 },
            new Aluno { Nome = "Henrique Alves",  Matricula = "2024008", Turma = "2B", DataNascimento = new DateOnly(2008, 6, 14),  Nota = 2.5 },
        };

        foreach (var a in alunos) repo.Add(a);
        Console.WriteLine("Base de dados criada com alunos de exemplo.\n");
    }
}
