namespace ApiClinica.DTOs;

public class RegisterDTO
{
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Senha { get; set; }
}
