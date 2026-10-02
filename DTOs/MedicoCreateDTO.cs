namespace ApiClinica.DTOs;

public class MedicoCreateDTO
{
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Telefone { get; set; }
    public required string CRM { get; set; }
}
