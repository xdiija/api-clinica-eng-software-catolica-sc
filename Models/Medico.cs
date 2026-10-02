using ApiClinica.Models.ValueObjects;

namespace ApiClinica.Models;

public class Medico
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required Email Email { get; set; }
    public required Telefone Telefone { get; set; }
    public required string CRM { get; set; }

    public List<Consulta> Consultas { get; set; } = [];
}
