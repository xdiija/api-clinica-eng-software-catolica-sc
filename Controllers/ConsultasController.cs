using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using ApiClinica.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private const int NenhumaConsulta = 0;

    private readonly AppDbContext _context;

    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/consultas
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        var consultas = await _context.Consultas.ToListAsync();

        var consultasDTO = consultas
            .Select(consulta => ConsultaMapper.ToDTO(consulta))
            .ToList();

        return Ok(consultasDTO);
    }

    // GET: api/consultas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta is null) return NotFound();

        return Ok(ConsultaMapper.ToDTO(consulta));
    }

    // POST: api/consultas
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {
        var consulta = ConsultaMapper.ToModel(dto);

        var erros = await ValidarAgendamento(
            NenhumaConsulta, consulta.PacienteId, consulta.MedicoId, consulta.DataHora);

        if (erros.Algum) return ValidationProblem(erros.ParaModelState());

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetConsultaById),
            new { id = consulta.Id },
            ConsultaMapper.ToDTO(consulta));
    }

    // PATCH: api/consultas/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta is null) return NotFound();

        ConsultaMapper.AplicarUpdate(dto, consulta);

        var erros = await ValidarAgendamento(
            consulta.Id, consulta.PacienteId, consulta.MedicoId, consulta.DataHora);

        if (erros.Algum)
        {
            await _context.Entry(consulta).ReloadAsync();
            return ValidationProblem(erros.ParaModelState());
        }

        await _context.SaveChangesAsync();

        return Ok(ConsultaMapper.ToDTO(consulta));
    }

    // DELETE: api/consultas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta is null) return NotFound();

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<ErrosDeValidacao> ValidarAgendamento(
        int idIgnorado, int pacienteId, int medicoId, JanelaConsulta janela)
    {
        var erros = new ErrosDeValidacao();

        if (!await _context.Pacientes.AnyAsync(paciente => paciente.Id == pacienteId))
            erros.Adicionar("pacienteId", "Paciente não encontrado.");

        if (!await _context.Medicos.AnyAsync(medico => medico.Id == medicoId))
            erros.Adicionar("medicoId", "Médico não encontrado.");

        if (janela.NoPassado(DateTime.Now))
            erros.Adicionar("dataHora", "Não é possível agendar consulta no passado.");

        if (erros.Algum) return erros;

        var candidatas = await _context.Consultas
            .Where(consulta => consulta.Id != idIgnorado
                && (consulta.MedicoId == medicoId || consulta.PacienteId == pacienteId))
            .ToListAsync();

        var conflitoDoMedico = candidatas.Any(consulta =>
            consulta.MedicoId == medicoId && janela.Sobrepoe(consulta.DataHora));

        var conflitoDoPaciente = candidatas.Any(consulta =>
            consulta.PacienteId == pacienteId && janela.Sobrepoe(consulta.DataHora));

        if (conflitoDoMedico)
            erros.Adicionar(
                "dataHora", "O médico já tem uma consulta que conflita com este horário.");

        if (conflitoDoPaciente)
            erros.Adicionar(
                "dataHora", "O paciente já tem uma consulta que conflita com este horário.");

        return erros;
    }
}
