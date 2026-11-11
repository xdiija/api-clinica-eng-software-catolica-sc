using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiClinica.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenDTO GerarToken(Usuario usuario)
    {
        var jwt = _configuration.GetSection("Jwt");

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var expiraEm = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpiraEmMinutos"));

        // Informações que vão dentro do token. A claim de Role é a usada pelo
        // [Authorize(Roles = "Admin")] para decidir quem pode fazer PATCH e DELETE.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email.Valor),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEm = expiraEm,
            Email = usuario.Email.Valor,
            Perfil = usuario.Perfil
        };
    }
}
