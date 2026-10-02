namespace ApiClinica.Models.ValueObjects;

public readonly record struct JanelaConsulta
{
    public static readonly TimeSpan Duracao = TimeSpan.FromMinutes(30);

    public DateTime Inicio { get; }

    private JanelaConsulta(DateTime inicio) => Inicio = inicio;

    public static JanelaConsulta De(DateTime inicio) => new(inicio);

    public DateTime Fim => Inicio + Duracao;

    public bool Sobrepoe(JanelaConsulta outra) => Inicio < outra.Fim && outra.Inicio < Fim;

    public bool NoPassado(DateTime agora) => Inicio < agora;

    public bool NoFuturo(DateTime agora) => Inicio > agora;

    public override string ToString() => Inicio.ToString("yyyy-MM-ddTHH:mm:ss");
}
