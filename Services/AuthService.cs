using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using ApiClinica.Models;
using ApiClinica.Models.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services;

public class AuthService : IAuthService
{
    private const int TamanhoMinimoSenha = 6;

    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        AppDbContext context,
        IPasswordHasher<Usuario> passwordHasher,
        ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Resultado<UsuarioReadDTO>> RegistrarAsync(RegisterDTO dto)
    {
        var erros = new ErrosDeValidacao();

        var nome = ValidacaoDeCampos.LerNome(dto.Nome, erros);
        var email = ValidacaoDeCampos.LerEmail(dto.Email, erros);

        if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < TamanhoMinimoSenha)
            erros.Adicionar("senha", $"Senha deve ter no mínimo {TamanhoMinimoSenha} caracteres.");

        if (erros.Algum) return Resultado<UsuarioReadDTO>.Invalido(erros);

        if (await _context.Usuarios.AnyAsync(u => u.Email == email!.Value))
        {
            erros.Adicionar("email", "Já existe um usuário cadastrado com este email.");
            return Resultado<UsuarioReadDTO>.Invalido(erros);
        }

        // Todo usuário registrado pela API é "Comum".
        // O Admin é criado na inicialização da aplicação (ver Data/SeedAdmin.cs).
        var usuario = new Usuario
        {
            Nome = nome!,
            Email = email!.Value,
            Perfil = Perfis.Comum
        };
        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, dto.Senha);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Resultado<UsuarioReadDTO>.Sucesso(new UsuarioReadDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email.Valor,
            Perfil = usuario.Perfil
        });
    }

    public async Task<TokenDTO?> LoginAsync(LoginDTO dto)
    {
        if (!Email.TryParse(dto.Email, out var email) || string.IsNullOrEmpty(dto.Senha))
            return null;

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario is null) return null;

        var verificacao = _passwordHasher.VerifyHashedPassword(
            usuario, usuario.SenhaHash, dto.Senha);

        if (verificacao == PasswordVerificationResult.Failed) return null;

        return _tokenService.GerarToken(usuario);
    }
}
