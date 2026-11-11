namespace ApiClinica.DTOs;

// Resposta do login.
public class TokenDTO
{
    public required string Token { get; set; }
    public required DateTime ExpiraEm { get; set; }
    public required string Email { get; set; }
    public required string Perfil { get; set; }
}
