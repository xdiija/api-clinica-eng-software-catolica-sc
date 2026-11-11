using ApiClinica.Mappers;

namespace ApiClinica.Services;

// Diz ao Controller o que aconteceu na camada de negócio,
// para ele escolher o status HTTP (200/201, 404 ou 400).
public enum StatusResultado
{
    Sucesso,
    NaoEncontrado,
    Invalido
}

public class Resultado
{
    public StatusResultado Status { get; }
    public ErrosDeValidacao? Erros { get; }

    protected Resultado(StatusResultado status, ErrosDeValidacao? erros)
    {
        Status = status;
        Erros = erros;
    }

    public static Resultado Sucesso() => new(StatusResultado.Sucesso, null);
    public static Resultado NaoEncontrado() => new(StatusResultado.NaoEncontrado, null);
    public static Resultado Invalido(ErrosDeValidacao erros) => new(StatusResultado.Invalido, erros);
}

public sealed class Resultado<T> : Resultado
{
    public T? Valor { get; }

    private Resultado(StatusResultado status, T? valor, ErrosDeValidacao? erros)
        : base(status, erros)
    {
        Valor = valor;
    }

    public static Resultado<T> Sucesso(T valor) =>
        new(StatusResultado.Sucesso, valor, null);

    public static new Resultado<T> NaoEncontrado() =>
        new(StatusResultado.NaoEncontrado, default, null);

    public static new Resultado<T> Invalido(ErrosDeValidacao erros) =>
        new(StatusResultado.Invalido, default, erros);
}
