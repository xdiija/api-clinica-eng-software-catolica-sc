using ApiClinica.Models.ValueObjects;

namespace ApiClinica.Models;

public class Paciente
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required Email Email { get; set; }
    public required Telefone Telefone { get; set; }
    public required DataNascimento DataNasc { get; set; }

    public required Cpf Cpf { get; init; }

    public List<Consulta> Consultas { get; set; } = [];
}
