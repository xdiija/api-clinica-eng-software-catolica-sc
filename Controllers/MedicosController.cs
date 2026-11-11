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
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;

    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        return Ok(await _service.ListarAsync());
    }

    // GET: api/medicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        var medico = await _service.BuscarPorIdAsync(id);

        if (medico is null) return NotFound();

        return Ok(medico);
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        var resultado = await _service.CriarAsync(dto);

        if (resultado.Status == StatusResultado.Invalido)
            return ValidationProblem(resultado.Erros!.ParaModelState());

        var medico = resultado.Valor!;

        return CreatedAtAction(nameof(GetMedicoById), new { id = medico.Id }, medico);
    }

    // PATCH: api/medicos/{id} — somente Admin
    [HttpPatch("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        var resultado = await _service.AtualizarAsync(id, dto);

        return resultado.Status switch
        {
            StatusResultado.NaoEncontrado => NotFound(),
            StatusResultado.Invalido => ValidationProblem(resultado.Erros!.ParaModelState()),
            _ => Ok(resultado.Valor)
        };
    }

    // DELETE: api/medicos/{id} — somente Admin
    [HttpDelete("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<IActionResult> DeleteMedico(int id)
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
