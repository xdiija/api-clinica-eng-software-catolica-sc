namespace ApiClinica.Models.ValueObjects;

public readonly record struct DataNascimento
{
    public DateOnly Valor { get; }

    private DataNascimento(DateOnly valor) => Valor = valor;

    public static bool TryCreate(DateOnly data, DateOnly hoje, out DataNascimento dataNasc)
    {
        dataNasc = default;

        if (data > hoje) return false;

        dataNasc = new DataNascimento(data);
        return true;
    }

    public static DataNascimento Reconstituir(DateOnly valor) => new(valor);

    public int Idade(DateOnly hoje)
    {
        var anos = hoje.Year - Valor.Year;

        if (Valor > hoje.AddYears(-anos)) anos--;

        return anos;
    }

    public override string ToString() => Valor.ToString("yyyy-MM-dd");
}
