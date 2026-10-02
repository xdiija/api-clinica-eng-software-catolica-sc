using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    private static DateOnly Hoje => DateOnly.FromDateTime(DateTime.Today);

    // GET: api/pacientes
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        var pacientesDTO = pacientes
            .Select(paciente => PacienteMapper.ToDTO(paciente, Hoje))
            .ToList();

        return Ok(pacientesDTO);
    }

    // GET: api/pacientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente is null) return NotFound();

        return Ok(PacienteMapper.ToDTO(paciente, Hoje));
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateDTO dto)
    {
        var erros = new ErrosDeValidacao();

        var paciente = PacienteMapper.ToModel(dto, Hoje, erros);

        if (paciente is null) return ValidationProblem(erros.ParaModelState());

        if (await _context.Pacientes.AnyAsync(p => p.Cpf == paciente.Cpf))
        {
            erros.Adicionar("cpf", "Já existe um paciente cadastrado com este CPF.");
            return ValidationProblem(erros.ParaModelState());
        }

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPacienteById),
            new { id = paciente.Id },
            PacienteMapper.ToDTO(paciente, Hoje));
    }

    // PATCH: api/pacientes/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente is null) return NotFound();

        var erros = new ErrosDeValidacao();

        PacienteMapper.AplicarUpdate(dto, paciente, Hoje, erros);

        if (erros.Algum) return ValidationProblem(erros.ParaModelState());

        await _context.SaveChangesAsync();

        return Ok(PacienteMapper.ToDTO(paciente, Hoje));
    }

    // DELETE: api/pacientes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente is null) return NotFound();

        var agora = DateTime.Now;

        var consultas = await _context.Consultas
            .Where(consulta => consulta.PacienteId == id)
            .ToListAsync();

        if (consultas.Any(consulta => consulta.DataHora.NoFuturo(agora)))
        {
            var erros = new ErrosDeValidacao();
            erros.Adicionar(
                "id", "Não é possível remover um paciente com consultas agendadas no futuro.");
            return ValidationProblem(erros.ParaModelState());
        }

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
