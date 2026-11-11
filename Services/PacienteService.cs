using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services;

public class PacienteService : IPacienteService
{
    private readonly AppDbContext _context;
    private readonly IPacienteMapper _mapper;

    // O mapper chega por Injeção de Dependências (registrado no Program.cs).
    public PacienteService(AppDbContext context, IPacienteMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private static DateOnly Hoje => DateOnly.FromDateTime(DateTime.Today);

    public async Task<List<PacienteReadDTO>> ListarAsync()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        return pacientes
            .Select(paciente => _mapper.ToDTO(paciente, Hoje))
            .ToList();
    }

    public async Task<PacienteReadDTO?> BuscarPorIdAsync(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        return paciente is null ? null : _mapper.ToDTO(paciente, Hoje);
    }

    public async Task<Resultado<PacienteReadDTO>> CriarAsync(PacienteCreateDTO dto)
    {
        var erros = new ErrosDeValidacao();

        var paciente = _mapper.ToModel(dto, Hoje, erros);

        if (paciente is null) return Resultado<PacienteReadDTO>.Invalido(erros);

        if (await _context.Pacientes.AnyAsync(p => p.Cpf == paciente.Cpf))
        {
            erros.Adicionar("cpf", "Já existe um paciente cadastrado com este CPF.");
            return Resultado<PacienteReadDTO>.Invalido(erros);
        }

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return Resultado<PacienteReadDTO>.Sucesso(_mapper.ToDTO(paciente, Hoje));
    }

    public async Task<Resultado<PacienteReadDTO>> AtualizarAsync(int id, PacienteUpdateDTO dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente is null) return Resultado<PacienteReadDTO>.NaoEncontrado();

        var erros = new ErrosDeValidacao();

        _mapper.AplicarUpdate(dto, paciente, Hoje, erros);

        if (erros.Algum) return Resultado<PacienteReadDTO>.Invalido(erros);

        await _context.SaveChangesAsync();

        return Resultado<PacienteReadDTO>.Sucesso(_mapper.ToDTO(paciente, Hoje));
    }

    public async Task<Resultado> RemoverAsync(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente is null) return Resultado.NaoEncontrado();

        var agora = DateTime.Now;

        var consultas = await _context.Consultas
            .Where(consulta => consulta.PacienteId == id)
            .ToListAsync();

        if (consultas.Any(consulta => consulta.DataHora.NoFuturo(agora)))
        {
            var erros = new ErrosDeValidacao();
            erros.Adicionar(
                "id", "Não é possível remover um paciente com consultas agendadas no futuro.");
            return Resultado.Invalido(erros);
        }

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return Resultado.Sucesso();
    }
}
