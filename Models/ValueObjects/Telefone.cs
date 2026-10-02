using System.Text.RegularExpressions;

namespace ApiClinica.Models.ValueObjects;

public readonly record struct Telefone
{
    private static readonly Regex Formato =
        new(@"^\(\d{2}\) \d{4,5}-\d{4}$", RegexOptions.Compiled);

    private readonly string? _valor;

    private Telefone(string valor) => _valor = valor;

    public string Valor => _valor ?? string.Empty;

    public static bool TryParse(string? entrada, out Telefone telefone)
    {
        telefone = default;

        if (entrada is null) return false;

        var aparado = entrada.Trim();

        if (!Formato.IsMatch(aparado)) return false;

        telefone = new Telefone(aparado);
        return true;
    }

    public static Telefone Reconstituir(string valor) => new(valor);

    public string Ddd => Valor.Length >= 3 ? Valor[1..3] : string.Empty;

    public string Numero => new(Valor.Skip(4).Where(char.IsDigit).ToArray());

    public override string ToString() => Valor;
}
