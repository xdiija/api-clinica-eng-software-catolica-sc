namespace ApiClinica.DTOs;

public class ConsultaCreateDTO
{
    public required int PacienteId { get; set; }
    public required int MedicoId { get; set; }
    public required DateTime DataHora { get; set; }
}
