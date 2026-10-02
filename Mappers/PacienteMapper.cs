using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Mappers;

public static class PacienteMapper
{
    public static Paciente? ToModel(
        PacienteCreateDTO dto, DateOnly hoje, ErrosDeValidacao erros)
    {
        var nome = ValidacaoDeCampos.LerNome(dto.Nome, erros);
        var email = ValidacaoDeCampos.LerEmail(dto.Email, erros);
        var telefone = ValidacaoDeCampos.LerTelefone(dto.Telefone, erros);
        var dataNasc = ValidacaoDeCampos.LerDataNascimento(dto.DataNasc, hoje, erros);
        var cpf = ValidacaoDeCampos.LerCpf(dto.Cpf, erros);

        if (erros.Algum) return null;

        return new Paciente
        {
            Nome = nome!,
            Email = email!.Value,
            Telefone = telefone!.Value,
            DataNasc = dataNasc!.Value,
            Cpf = cpf!.Value
        };
    }

    public static void AplicarUpdate(
        PacienteUpdateDTO dto, Paciente paciente, DateOnly hoje, ErrosDeValidacao erros)
    {
        if (dto.Cpf is not null)
            erros.Adicionar("cpf", "O CPF não pode ser alterado.");

        var nome = dto.Nome is null ? null : ValidacaoDeCampos.LerNome(dto.Nome, erros);
        var email = dto.Email is null ? null : ValidacaoDeCampos.LerEmail(dto.Email, erros);
        var telefone = dto.Telefone is null
            ? null
            : ValidacaoDeCampos.LerTelefone(dto.Telefone, erros);
        var dataNasc = dto.DataNasc is null
            ? null
            : ValidacaoDeCampos.LerDataNascimento(dto.DataNasc.Value, hoje, erros);

        if (erros.Algum) return;

        if (nome is not null) paciente.Nome = nome;
        if (email is not null) paciente.Email = email.Value;
        if (telefone is not null) paciente.Telefone = telefone.Value;
        if (dataNasc is not null) paciente.DataNasc = dataNasc.Value;
    }

    public static PacienteReadDTO ToDTO(Paciente paciente, DateOnly hoje)
    {
        return new PacienteReadDTO
        {
            Id = paciente.Id,
            Nome = paciente.Nome,
            Email = paciente.Email.Valor,
            Telefone = paciente.Telefone.Valor,
            DataNasc = paciente.DataNasc.Valor,
            Cpf = paciente.Cpf.Formatado,
            Idade = paciente.DataNasc.Idade(hoje)
        };
    }
}
