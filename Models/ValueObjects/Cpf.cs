namespace ApiClinica.Models.ValueObjects;

public readonly record struct Cpf
{
    private const int Tamanho = 11;

    private readonly string? _valor;

    private Cpf(string valor) => _valor = valor;

    public string Valor => _valor ?? string.Empty;

    public static bool TryParse(string? entrada, out Cpf cpf)
    {
        cpf = default;

        if (entrada is null) return false;

        var digitos = new string(entrada.Where(char.IsDigit).ToArray());

        if (digitos.Length != Tamanho) return false;

        if (digitos.Distinct().Count() == 1) return false;

        if (DigitoVerificador(digitos[..9]) != digitos[9] - '0') return false;
        if (DigitoVerificador(digitos[..10]) != digitos[10] - '0') return false;

        cpf = new Cpf(digitos);
        return true;
    }

    public static Cpf Reconstituir(string valor) => new(valor);

    private static int DigitoVerificador(string baseCpf)
    {
        var soma = 0;
        var peso = baseCpf.Length + 1;

        foreach (var c in baseCpf)
        {
            soma += (c - '0') * peso;
            peso--;
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    public string Formatado => $"{Valor[..3]}.{Valor[3..6]}.{Valor[6..9]}-{Valor[9..]}";

    public override string ToString() => Valor;
}
