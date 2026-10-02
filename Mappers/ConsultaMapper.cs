using ApiClinica.DTOs;
using ApiClinica.Models;
using ApiClinica.Models.ValueObjects;

namespace ApiClinica.Mappers;

public static class ConsultaMapper
{
    public static Consulta ToModel(ConsultaCreateDTO dto)
    {
        return new Consulta
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHora = JanelaConsulta.De(dto.DataHora)
        };
    }

    public static void AplicarUpdate(ConsultaUpdateDTO dto, Consulta consulta)
    {
        if (dto.PacienteId is not null) consulta.PacienteId = dto.PacienteId.Value;
        if (dto.MedicoId is not null) consulta.MedicoId = dto.MedicoId.Value;
        if (dto.DataHora is not null) consulta.DataHora = JanelaConsulta.De(dto.DataHora.Value);
    }

    public static ConsultaReadDTO ToDTO(Consulta consulta)
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
