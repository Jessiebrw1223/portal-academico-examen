using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.Services;

public interface ICursoCacheService
{
    Task<List<Curso>> GetCursosActivosAsync();
    Task InvalidarCursosActivosAsync();
}