using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.Services;

public interface IHorarioValidator
{
    bool SeSolapan(Curso a, Curso b);
}
