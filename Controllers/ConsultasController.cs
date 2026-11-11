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
public class ConsultasController : ControllerBase
{
    private readonly IConsultaService _service;

    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }

    // GET: api/consultas
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        return Ok(await _service.ListarAsync());
    }

    // GET: api/consultas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var consulta = await _service.BuscarPorIdAsync(id);

        if (consulta is null) return NotFound();

        return Ok(consulta);
    }

    // POST: api/consultas
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {
        var resultado = await _service.CriarAsync(dto);

        if (resultado.Status == StatusResultado.Invalido)
            return ValidationProblem(resultado.Erros!.ParaModelState());

        var consulta = resultado.Valor!;

        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consulta);
    }

    // PATCH: api/consultas/{id} — somente Admin
    [HttpPatch("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        var resultado = await _service.AtualizarAsync(id, dto);

        return resultado.Status switch
        {
            StatusResultado.NaoEncontrado => NotFound(),
            StatusResultado.Invalido => ValidationProblem(resultado.Erros!.ParaModelState()),
            _ => Ok(resultado.Valor)
        };
    }

    // DELETE: api/consultas/{id} — somente Admin
    [HttpDelete("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        var resultado = await _service.RemoverAsync(id);

        return resultado.Status switch
        {
            StatusResultado.NaoEncontrado => NotFound(),
            _ => NoContent()
        };
    }
}
