namespace ApiClinica.DTOs;

public class ConsultaUpdateDTO
{
    public int? PacienteId { get; set; }
    public int? MedicoId { get; set; }
    public DateTime? DataHora { get; set; }
}
