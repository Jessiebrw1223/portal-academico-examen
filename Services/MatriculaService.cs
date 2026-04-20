using Microsoft.EntityFrameworkCore;
using PortalAcademicoExamen.Data;
using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.Services;

public class MatriculaService : IMatriculaService
{
    private readonly ApplicationDbContext _context;
    private readonly IHorarioValidator _horarioValidator;

    public MatriculaService(ApplicationDbContext context, IHorarioValidator horarioValidator)
    {
        _context = context;
        _horarioValidator = horarioValidator;
    }

    public async Task<(bool ok, string mensaje)> CrearMatriculaPendienteAsync(int cursoId, string usuarioId)
    {
        var curso = await _context.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);

        if (curso is null)
            return (false, "El curso no existe o está inactivo.");

        if (curso.Creditos <= 0)
            return (false, "El curso no tiene créditos válidos.");

        if (curso.HorarioInicio >= curso.HorarioFin)
            return (false, "El curso tiene un horario inválido.");

        var yaMatriculado = await _context.Matriculas
            .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == usuarioId && m.Estado != EstadoMatricula.Cancelada);

        if (yaMatriculado)
            return (false, "Ya estás matriculado en este curso.");

        var ocupadas = await _context.Matriculas
            .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);

        if (ocupadas >= curso.CupoMaximo)
            return (false, "No hay cupos disponibles para este curso.");

        var cursosDelUsuario = await _context.Matriculas
            .Include(m => m.Curso)
            .Where(m => m.UsuarioId == usuarioId && m.Estado != EstadoMatricula.Cancelada)
            .ToListAsync();

        if (cursosDelUsuario.Any(m => m.Curso is not null && _horarioValidator.SeSolapan(m.Curso, curso)))
            return (false, "El horario del curso se solapa con otra matrícula vigente.");

        _context.Matriculas.Add(new Matricula
        {
            CursoId = cursoId,
            UsuarioId = usuarioId,
            Estado = EstadoMatricula.Pendiente,
            FechaRegistro = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Matrícula registrada en estado Pendiente.");
    }

    public async Task<(bool ok, string mensaje)> CambiarEstadoAsync(int matriculaId, string estado)
    {
        var matricula = await _context.Matriculas
            .Include(m => m.Curso)
            .FirstOrDefaultAsync(m => m.Id == matriculaId);

        if (matricula is null)
            return (false, "Matrícula no encontrada.");

        if (!Enum.TryParse<EstadoMatricula>(estado, true, out var nuevoEstado))
            return (false, "Estado inválido.");

        if (nuevoEstado == EstadoMatricula.Confirmada && matricula.Curso is not null)
        {
            var ocupadas = await _context.Matriculas
                .CountAsync(m => m.CursoId == matricula.CursoId && m.Estado == EstadoMatricula.Confirmada && m.Id != matricula.Id);

            if (ocupadas >= matricula.Curso.CupoMaximo)
                return (false, "No se puede confirmar: el curso ya alcanzó su cupo máximo.");
        }

        matricula.Estado = nuevoEstado;
        await _context.SaveChangesAsync();
        return (true, $"Matrícula actualizada a {nuevoEstado}.");
    }
}
