namespace LinqCrud.Models;

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Matricula { get; set; } = "";
    public string Turma { get; set; } = "";
    public DateOnly DataNascimento { get; set; }
    public double Nota { get; set; }
}
