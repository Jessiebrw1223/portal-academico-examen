namespace PortalAcademicoExamen.ViewModels;

public class MatricularViewModel
{
    public int CursoId { get; set; }
    public string CursoNombre { get; set; } = string.Empty;
    public string CursoCodigo { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public int CupoDisponible { get; set; }
    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }
    public string? Mensaje { get; set; }
    public bool Exito { get; set; }
}
