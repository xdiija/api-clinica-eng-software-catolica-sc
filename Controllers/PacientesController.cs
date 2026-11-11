using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Models;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos os métodos exigem token JWT válido.
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    // O service chega por Injeção de Dependências (registrado no Program.cs).
    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    // GET: api/pacientes
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        return Ok(await _service.ListarAsync());
    }

    // GET: api/pacientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        var paciente = await _service.BuscarPorIdAsync(id);

        if (paciente is null) return NotFound();

        return Ok(paciente);
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateDTO dto)
    {
        var resultado = await _service.CriarAsync(dto);

        if (resultado.Status == StatusResultado.Invalido)
            return ValidationProblem(resultado.Erros!.ParaModelState());

        var paciente = resultado.Valor!;

        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, paciente);
    }

    // PATCH: api/pacientes/{id} — somente Admin
    [HttpPatch("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        var resultado = await _service.AtualizarAsync(id, dto);

        return resultado.Status switch
        {
            StatusResultado.NaoEncontrado => NotFound(),
            StatusResultado.Invalido => ValidationProblem(resultado.Erros!.ParaModelState()),
            _ => Ok(resultado.Valor)
        };
    }

    // DELETE: api/pacientes/{id} — somente Admin
    [HttpDelete("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        var resultado = await _service.RemoverAsync(id);

        return resultado.Status switch
        {
            StatusResultado.NaoEncontrado => NotFound(),
            StatusResultado.Invalido => ValidationProblem(resultado.Erros!.ParaModelState()),
            _ => NoContent()
        };
    }
}
