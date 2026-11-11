using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Interfaces;

public interface IAuthService
{
    Task<Resultado<UsuarioReadDTO>> RegistrarAsync(RegisterDTO dto);

    // Retorna null quando email ou senha estão errados.
    Task<TokenDTO?> LoginAsync(LoginDTO dto);
}
