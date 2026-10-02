namespace ApiClinica.DTOs;

public class ConsultaReadDTO
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public DateTime DataHora { get; set; }

    public DateTime DataHoraFim { get; set; }
}
