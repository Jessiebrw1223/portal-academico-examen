using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.ViewModels;

public class CursoCatalogoFiltroViewModel
{
    public string? Nombre { get; set; }
    public int? CreditosMin { get; set; }
    public int? CreditosMax { get; set; }
    public TimeSpan? HorarioInicio { get; set; }
    public TimeSpan? HorarioFin { get; set; }
    public List<Curso> Cursos { get; set; } = new();
}
