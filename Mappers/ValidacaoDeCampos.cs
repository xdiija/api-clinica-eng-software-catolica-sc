using ApiClinica.Models.ValueObjects;

namespace ApiClinica.Mappers;

internal static class ValidacaoDeCampos
{
    private const int TamanhoMaximoNome = 120;
    private const int TamanhoMaximoCrm = 20;

    public static string? LerNome(string? valor, ErrosDeValidacao erros)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            erros.Adicionar("nome", "Nome é obrigatório.");
            return null;
        }

        var aparado = valor.Trim();

        if (aparado.Length > TamanhoMaximoNome)
        {
            erros.Adicionar("nome", $"Nome deve ter no máximo {TamanhoMaximoNome} caracteres.");
            return null;
        }

        return aparado;
    }

    public static Email? LerEmail(string? valor, ErrosDeValidacao erros)
    {
        if (Email.TryParse(valor, out var email)) return email;

        erros.Adicionar("email", "Email inválido.");
        return null;
    }

    public static Telefone? LerTelefone(string? valor, ErrosDeValidacao erros)
    {
        if (Telefone.TryParse(valor, out var telefone)) return telefone;

        erros.Adicionar("telefone", "Telefone inválido. Formato esperado: (47) 98888-7777.");
        return null;
    }

    public static Cpf? LerCpf(string? valor, ErrosDeValidacao erros)
    {
        if (Cpf.TryParse(valor, out var cpf)) return cpf;

        erros.Adicionar("cpf", "CPF inválido.");
        return null;
    }

    public static DataNascimento? LerDataNascimento(
        DateOnly valor, DateOnly hoje, ErrosDeValidacao erros)
    {
        if (DataNascimento.TryCreate(valor, hoje, out var dataNasc)) return dataNasc;

        erros.Adicionar("dataNasc", "Data de nascimento não pode ser no futuro.");
        return null;
    }

    public static string? LerCrm(string? valor, ErrosDeValidacao erros)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            erros.Adicionar("crm", "CRM é obrigatório.");
            return null;
        }

        var aparado = valor.Trim().ToUpperInvariant();

        if (aparado.Length > TamanhoMaximoCrm)
        {
            erros.Adicionar("crm", $"CRM deve ter no máximo {TamanhoMaximoCrm} caracteres.");
            return null;
        }

        return aparado;
    }
}
