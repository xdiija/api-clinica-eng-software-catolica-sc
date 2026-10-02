namespace ApiClinica.DTOs;

public class PacienteUpdateDTO
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateOnly? DataNasc { get; set; }

    public string? Cpf { get; set; }
}
