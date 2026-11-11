using ApiClinica.DTOs;
using ApiClinica.Mappers;
using ApiClinica.Models;

namespace ApiClinica.Interfaces;

public interface IMedicoMapper
{
    Medico? ToModel(MedicoCreateDTO dto, ErrosDeValidacao erros);

    void AplicarUpdate(MedicoUpdateDTO dto, Medico medico, ErrosDeValidacao erros);

    MedicoReadDTO ToDTO(Medico medico);
}
