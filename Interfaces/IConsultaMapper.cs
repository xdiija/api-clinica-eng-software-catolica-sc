using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces;

public interface IConsultaMapper
{
    Consulta ToModel(ConsultaCreateDTO dto);

    void AplicarUpdate(ConsultaUpdateDTO dto, Consulta consulta);

    ConsultaReadDTO ToDTO(Consulta consulta);
}
