namespace ApiClinica.DTOs;

public class MedicoReadDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Telefone { get; set; } = default!;
    public string CRM { get; set; } = default!;
}
