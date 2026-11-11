using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services;

public class MedicoService : IMedicoService
{
    private readonly AppDbContext _context;
    private readonly IMedicoMapper _mapper;

    public MedicoService(AppDbContext context, IMedicoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<MedicoReadDTO>> ListarAsync()
    {
        var medicos = await _context.Medicos.ToListAsync();

        return medicos
            .Select(medico => _mapper.ToDTO(medico))
            .ToList();
    }

    public async Task<MedicoReadDTO?> BuscarPorIdAsync(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        return medico is null ? null : _mapper.ToDTO(medico);
    }

    public async Task<Resultado<MedicoReadDTO>> CriarAsync(MedicoCreateDTO dto)
    {
        var erros = new ErrosDeValidacao();

        var medico = _mapper.ToModel(dto, erros);

        if (medico is null) return Resultado<MedicoReadDTO>.Invalido(erros);

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return Resultado<MedicoReadDTO>.Sucesso(_mapper.ToDTO(medico));
    }

    public async Task<Resultado<MedicoReadDTO>> AtualizarAsync(int id, MedicoUpdateDTO dto)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico is null) return Resultado<MedicoReadDTO>.NaoEncontrado();

        var erros = new ErrosDeValidacao();

        _mapper.AplicarUpdate(dto, medico, erros);

        if (erros.Algum) return Resultado<MedicoReadDTO>.Invalido(erros);

        await _context.SaveChangesAsync();

        return Resultado<MedicoReadDTO>.Sucesso(_mapper.ToDTO(medico));
    }

    public async Task<Resultado> RemoverAsync(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico is null) return Resultado.NaoEncontrado();

        var agora = DateTime.Now;

        var consultas = await _context.Consultas
            .Where(consulta => consulta.MedicoId == id)
            .ToListAsync();

        if (consultas.Any(consulta => consulta.DataHora.NoFuturo(agora)))
        {
            var erros = new ErrosDeValidacao();
            erros.Adicionar(
                "id", "Não é possível remover um médico com consultas agendadas no futuro.");
            return Resultado.Invalido(erros);
        }

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();

        return Resultado.Sucesso();
    }
}
