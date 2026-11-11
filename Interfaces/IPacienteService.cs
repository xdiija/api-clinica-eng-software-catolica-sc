using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Interfaces;

public interface IPacienteService
{
    Task<List<PacienteReadDTO>> ListarAsync();
    Task<PacienteReadDTO?> BuscarPorIdAsync(int id);
    Task<Resultado<PacienteReadDTO>> CriarAsync(PacienteCreateDTO dto);
    Task<Resultado<PacienteReadDTO>> AtualizarAsync(int id, PacienteUpdateDTO dto);
    Task<Resultado> RemoverAsync(int id);
}
