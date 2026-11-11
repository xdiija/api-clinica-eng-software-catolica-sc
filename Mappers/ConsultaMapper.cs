using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Models;
using ApiClinica.Models.ValueObjects;

namespace ApiClinica.Mappers;

public class ConsultaMapper : IConsultaMapper
{
    public Consulta ToModel(ConsultaCreateDTO dto)
    {
        return new Consulta
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHora = JanelaConsulta.De(dto.DataHora)
        };
    }

    public void AplicarUpdate(ConsultaUpdateDTO dto, Consulta consulta)
    {
        if (dto.PacienteId is not null) consulta.PacienteId = dto.PacienteId.Value;
        if (dto.MedicoId is not null) consulta.MedicoId = dto.MedicoId.Value;
        if (dto.DataHora is not null) consulta.DataHora = JanelaConsulta.De(dto.DataHora.Value);
    }

    public ConsultaReadDTO ToDTO(Consulta consulta)
    {
        return new ConsultaReadDTO
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            MedicoId = consulta.MedicoId,
            DataHora = consulta.DataHora.Inicio,
            DataHoraFim = consulta.DataHora.Fim
        };
    }
}
