using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Registro e login precisam funcionar sem token.
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var resultado = await _service.RegistrarAsync(dto);

        if (resultado.Status == StatusResultado.Invalido)
            return ValidationProblem(resultado.Erros!.ParaModelState());

        return StatusCode(StatusCodes.Status201Created, resultado.Valor);
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var token = await _service.LoginAsync(dto);

        if (token is null) return Unauthorized(new { mensagem = "Email ou senha inválidos." });

        return Ok(token);
    }
}
