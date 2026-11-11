using ApiClinica.DTOs;
using ApiClinica.Mappers;
using ApiClinica.Models;

namespace ApiClinica.Interfaces;

public interface IPacienteMapper
{
    Paciente? ToModel(PacienteCreateDTO dto, DateOnly hoje, ErrosDeValidacao erros);

    void AplicarUpdate(
        PacienteUpdateDTO dto, Paciente paciente, DateOnly hoje, ErrosDeValidacao erros);

    PacienteReadDTO ToDTO(Paciente paciente, DateOnly hoje);
}
