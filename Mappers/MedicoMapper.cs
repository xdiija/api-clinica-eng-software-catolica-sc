using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class MedicoMapper
{
    public static Medico? ToModel(MedicoCreateDTO dto, ErrosDeValidacao erros)
    {
        var nome = ValidacaoDeCampos.LerNome(dto.Nome, erros);
        var email = ValidacaoDeCampos.LerEmail(dto.Email, erros);
        var telefone = ValidacaoDeCampos.LerTelefone(dto.Telefone, erros);
        var crm = ValidacaoDeCampos.LerCrm(dto.CRM, erros);

        if (erros.Algum) return null;

        return new Medico
        {
            Nome = nome!,
            Email = email!.Value,
            Telefone = telefone!.Value,
            CRM = crm!
        };
    }

    public static void AplicarUpdate(MedicoUpdateDTO dto, Medico medico, ErrosDeValidacao erros)
    {
        var nome = dto.Nome is null ? null : ValidacaoDeCampos.LerNome(dto.Nome, erros);
        var email = dto.Email is null ? null : ValidacaoDeCampos.LerEmail(dto.Email, erros);
        var telefone = dto.Telefone is null
            ? null
            : ValidacaoDeCampos.LerTelefone(dto.Telefone, erros);
        var crm = dto.CRM is null ? null : ValidacaoDeCampos.LerCrm(dto.CRM, erros);

        if (erros.Algum) return;

        if (nome is not null) medico.Nome = nome;
        if (email is not null) medico.Email = email.Value;
        if (telefone is not null) medico.Telefone = telefone.Value;
        if (crm is not null) medico.CRM = crm;
    }

    public static MedicoReadDTO ToDTO(Medico medico)
    {
        return new MedicoReadDTO
        {
            Id = medico.Id,
            Nome = medico.Nome,
            Email = medico.Email.Valor,
            Telefone = medico.Telefone.Valor,
            CRM = medico.CRM
        };
    }
}
