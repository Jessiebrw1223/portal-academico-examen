using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.Services;

public class HorarioValidator : IHorarioValidator
{
    public bool SeSolapan(Curso a, Curso b)
    {
        return a.HorarioInicio < b.HorarioFin && b.HorarioInicio < a.HorarioFin;
    }
}
