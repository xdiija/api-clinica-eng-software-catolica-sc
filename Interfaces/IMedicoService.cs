using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Interfaces;

public interface IMedicoService
{
    Task<List<MedicoReadDTO>> ListarAsync();
    Task<MedicoReadDTO?> BuscarPorIdAsync(int id);
    Task<Resultado<MedicoReadDTO>> CriarAsync(MedicoCreateDTO dto);
    Task<Resultado<MedicoReadDTO>> AtualizarAsync(int id, MedicoUpdateDTO dto);
    Task<Resultado> RemoverAsync(int id);
}
