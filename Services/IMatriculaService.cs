namespace PortalAcademicoExamen.Services;

public interface IMatriculaService
{
    Task<(bool ok, string mensaje)> CrearMatriculaPendienteAsync(int cursoId, string usuarioId);
    Task<(bool ok, string mensaje)> CambiarEstadoAsync(int matriculaId, string estado);
}
