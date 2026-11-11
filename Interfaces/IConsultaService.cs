using ApiClinica.DTOs;
using ApiClinica.Services;

namespace ApiClinica.Interfaces;

public interface IConsultaService
{
    Task<List<ConsultaReadDTO>> ListarAsync();
    Task<ConsultaReadDTO?> BuscarPorIdAsync(int id);
    Task<Resultado<ConsultaReadDTO>> CriarAsync(ConsultaCreateDTO dto);
    Task<Resultado<ConsultaReadDTO>> AtualizarAsync(int id, ConsultaUpdateDTO dto);
    Task<Resultado> RemoverAsync(int id);
}
