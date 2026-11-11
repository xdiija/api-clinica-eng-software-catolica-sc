using ApiClinica.Models.ValueObjects;

namespace ApiClinica.Models;

public class Usuario
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required Email Email { get; set; }

    // Nunca guardamos a senha em texto puro, só o hash (PasswordHasher do ASP.NET Core).
    public string SenhaHash { get; set; } = string.Empty;

    // "Comum" ou "Admin" — vira a claim de role dentro do token JWT.
    public string Perfil { get; set; } = Perfis.Comum;
}

// Os dois tipos de usuário do sistema.
public static class Perfis
{
    public const string Comum = "Comum";
    public const string Admin = "Admin";
}
