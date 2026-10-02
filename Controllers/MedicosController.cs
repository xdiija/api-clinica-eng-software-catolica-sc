using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var medicos = await _context.Medicos.ToListAsync();

        var medicosDTO = medicos
            .Select(medico => MedicoMapper.ToDTO(medico))
            .ToList();

        return Ok(medicosDTO);
    }

    // GET: api/medicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico is null) return NotFound();

        return Ok(MedicoMapper.ToDTO(medico));
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        var erros = new ErrosDeValidacao();

        var medico = MedicoMapper.ToModel(dto, erros);

        if (medico is null) return ValidationProblem(erros.ParaModelState());

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMedicoById),
            new { id = medico.Id },
            MedicoMapper.ToDTO(medico));
    }

    // PATCH: api/medicos/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico is null) return NotFound();

        var erros = new ErrosDeValidacao();

        MedicoMapper.AplicarUpdate(dto, medico, erros);

        if (erros.Algum) return ValidationProblem(erros.ParaModelState());

        await _context.SaveChangesAsync();

        return Ok(MedicoMapper.ToDTO(medico));
    }

    // DELETE: api/medicos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico is null) return NotFound();

        var agora = DateTime.Now;

        var consultas = await _context.Consultas
            .Where(consulta => consulta.MedicoId == id)
            .ToListAsync();

        if (consultas.Any(consulta => consulta.DataHora.NoFuturo(agora)))
        {
            var erros = new ErrosDeValidacao();
            erros.Adicionar(
                "id", "Não é possível remover um médico com consultas agendadas no futuro.");
            return ValidationProblem(erros.ParaModelState());
        }

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
