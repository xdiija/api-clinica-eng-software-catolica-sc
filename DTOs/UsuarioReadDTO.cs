namespace ApiClinica.DTOs;

// Resposta do registro — nunca devolve a senha nem o hash.
public class UsuarioReadDTO
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Perfil { get; set; }
}
