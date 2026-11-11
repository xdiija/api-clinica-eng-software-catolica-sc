using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces;

public interface ITokenService
{
    TokenDTO GerarToken(Usuario usuario);
}
