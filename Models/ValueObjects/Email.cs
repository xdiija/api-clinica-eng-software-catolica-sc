using System.Text.RegularExpressions;

namespace ApiClinica.Models.ValueObjects;

public readonly record struct Email
{
    private static readonly Regex Formato =
        new(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)+$", RegexOptions.Compiled);

    private const int TamanhoMaximo = 254;

    private readonly string? _valor;

    private Email(string valor) => _valor = valor;

    public string Valor => _valor ?? string.Empty;

    public static bool TryParse(string? entrada, out Email email)
    {
        email = default;

        if (string.IsNullOrWhiteSpace(entrada)) return false;

        var normalizado = entrada.Trim().ToLowerInvariant();

        if (normalizado.Length > TamanhoMaximo) return false;
        if (!Formato.IsMatch(normalizado)) return false;

        email = new Email(normalizado);
        return true;
    }

    public static Email Reconstituir(string valor) => new(valor);

    public string Dominio => Valor[(Valor.IndexOf('@') + 1)..];

    public override string ToString() => Valor;
}
