using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using ApiClinica.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services;

public class ConsultaService : IConsultaService
{
    private const int NenhumaConsulta = 0;

    private readonly AppDbContext _context;
    private readonly IConsultaMapper _mapper;

    public ConsultaService(AppDbContext context, IConsultaMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ConsultaReadDTO>> ListarAsync()
    {
        var consultas = await _context.Consultas.ToListAsync();

        return consultas
            .Select(consulta => _mapper.ToDTO(consulta))
            .ToList();
    }

    public async Task<ConsultaReadDTO?> BuscarPorIdAsync(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        return consulta is null ? null : _mapper.ToDTO(consulta);
    }

    public async Task<Resultado<ConsultaReadDTO>> CriarAsync(ConsultaCreateDTO dto)
    {
        var consulta = _mapper.ToModel(dto);

        var erros = await ValidarAgendamento(
            NenhumaConsulta, consulta.PacienteId, consulta.MedicoId, consulta.DataHora);

        if (erros.Algum) return Resultado<ConsultaReadDTO>.Invalido(erros);

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        return Resultado<ConsultaReadDTO>.Sucesso(_mapper.ToDTO(consulta));
    }

    public async Task<Resultado<ConsultaReadDTO>> AtualizarAsync(int id, ConsultaUpdateDTO dto)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta is null) return Resultado<ConsultaReadDTO>.NaoEncontrado();

        _mapper.AplicarUpdate(dto, consulta);

        // Valida de novo com os valores finais: se o paciente, o médico ou o horário
        // mudaram, as regras de conflito precisam ser checadas outra vez.
        var erros = await ValidarAgendamento(
            consulta.Id, consulta.PacienteId, consulta.MedicoId, consulta.DataHora);

        if (erros.Algum)
        {
            // Desfaz as alterações em memória para não salvar dados inválidos.
            await _context.Entry(consulta).ReloadAsync();
            return Resultado<ConsultaReadDTO>.Invalido(erros);
        }

        await _context.SaveChangesAsync();

        return Resultado<ConsultaReadDTO>.Sucesso(_mapper.ToDTO(consulta));
    }

    public async Task<Resultado> RemoverAsync(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta is null) return Resultado.NaoEncontrado();

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        return Resultado.Sucesso();
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
