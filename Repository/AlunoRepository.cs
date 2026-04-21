using System.Text.Json;
using System.Text.Json.Serialization;
using LinqCrud.Models;

namespace LinqCrud.Repository;

public class AlunoRepository
{
    private const string FilePath = "alunos.json";
    private List<Aluno> _alunos;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public AlunoRepository()
    {
        _alunos = Load();
    }

    // ── CREATE ────────────────────────────────────────────────────────────────

    public Aluno Add(Aluno aluno)
    {
        aluno.Id = _alunos.Any() ? _alunos.Max(a => a.Id) + 1 : 1;
        _alunos.Add(aluno);
        Save();
        return aluno;
    }

    // ── READ (LINQ) ───────────────────────────────────────────────────────────

    public IEnumerable<Aluno> GetAll() => _alunos;

    public Aluno? GetById(int id) =>
        _alunos.FirstOrDefault(a => a.Id == id);

    public IEnumerable<Aluno> GetByTurma(string turma) =>
        _alunos.Where(a => a.Turma.Equals(turma, StringComparison.OrdinalIgnoreCase))
               .OrderBy(a => a.Nome);

    public IEnumerable<Aluno> GetByFaixaNota(double min, double max) =>
        _alunos.Where(a => a.Nota >= min && a.Nota <= max)
               .OrderByDescending(a => a.Nota);

    public IEnumerable<Aluno> GetAprovados(double mediaMinima = 5.0) =>
        _alunos.Where(a => a.Nota >= mediaMinima)
               .OrderByDescending(a => a.Nota);

    public IEnumerable<Aluno> GetReprovados(double mediaMinima = 5.0) =>
        _alunos.Where(a => a.Nota < mediaMinima)
               .OrderBy(a => a.Nota);

    public IEnumerable<IGrouping<string, Aluno>> GroupByTurma() =>
        _alunos.GroupBy(a => a.Turma).OrderBy(g => g.Key);

    public IEnumerable<Aluno> Search(string termo) =>
        _alunos.Where(a =>
            a.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
            a.Matricula.Contains(termo, StringComparison.OrdinalIgnoreCase));

    public double GetMediaGeral() =>
        _alunos.Any() ? _alunos.Average(a => a.Nota) : 0;

    public Aluno? GetMelhorAluno() =>
        _alunos.MaxBy(a => a.Nota);

    // ── UPDATE ────────────────────────────────────────────────────────────────

    public bool Update(int id, Action<Aluno> update)
    {
        var aluno = _alunos.FirstOrDefault(a => a.Id == id);
        if (aluno is null) return false;
        update(aluno);
        Save();
        return true;
    }

    // ── DELETE ────────────────────────────────────────────────────────────────

    public bool Delete(int id)
    {
        var removed = _alunos.RemoveAll(a => a.Id == id) > 0;
        if (removed) Save();
        return removed;
    }

    // ── PERSISTENCE ───────────────────────────────────────────────────────────

    private List<Aluno> Load()
    {
        if (!File.Exists(FilePath)) return [];
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Aluno>>(json, JsonOpts) ?? [];
    }

    private void Save()
    {
        File.WriteAllText(FilePath, JsonSerializer.Serialize(_alunos, JsonOpts));
    }
}
