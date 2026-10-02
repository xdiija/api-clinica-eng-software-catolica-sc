namespace ApiClinica.DTOs;

public class PacienteReadDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Telefone { get; set; } = default!;
    public DateOnly DataNasc { get; set; }

    public string Cpf { get; set; } = default!;

    public int Idade { get; set; }
}
