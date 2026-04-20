using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoExamen.Data;
using PortalAcademicoExamen.Models;
using PortalAcademicoExamen.Services;
using PortalAcademicoExamen.ViewModels;

namespace PortalAcademicoExamen.Controllers;

[Authorize(Roles = "Coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICursoCacheService _cursoCacheService;
    private readonly IMatriculaService _matriculaService;

    public CoordinadorController(ApplicationDbContext context, ICursoCacheService cursoCacheService, IMatriculaService matriculaService)
    {
        _context = context;
        _cursoCacheService = cursoCacheService;
        _matriculaService = matriculaService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new CoordinadorDashboardViewModel
        {
            Cursos = await _context.Cursos.OrderBy(c => c.Nombre).ToListAsync()
        };
        return View(vm);
    }

    public IActionResult CreateCourse() => View(new Curso { Activo = true });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse(Curso curso)
    {
        if (curso.Creditos <= 0)
            ModelState.AddModelError(nameof(curso.Creditos), "Los créditos deben ser mayores a 0.");
        if (curso.HorarioFin <= curso.HorarioInicio)
            ModelState.AddModelError(nameof(curso.HorarioFin), "HorarioFin no puede ser anterior o igual a HorarioInicio.");
        if (await _context.Cursos.AnyAsync(c => c.Codigo == curso.Codigo))
            ModelState.AddModelError(nameof(curso.Codigo), "El código del curso ya existe.");

        if (!ModelState.IsValid)
            return View(curso);

        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();
        await _cursoCacheService.InvalidarCursosActivosAsync();
        TempData["Success"] = "Curso creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EditCourse(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        return curso is null ? NotFound() : View(curso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourse(Curso curso)
    {
        if (curso.Creditos <= 0)
            ModelState.AddModelError(nameof(curso.Creditos), "Los créditos deben ser mayores a 0.");
        if (curso.HorarioFin <= curso.HorarioInicio)
            ModelState.AddModelError(nameof(curso.HorarioFin), "HorarioFin no puede ser anterior o igual a HorarioInicio.");
        if (await _context.Cursos.AnyAsync(c => c.Codigo == curso.Codigo && c.Id != curso.Id))
            ModelState.AddModelError(nameof(curso.Codigo), "El código del curso ya existe.");

        if (!ModelState.IsValid)
            return View(curso);

        _context.Cursos.Update(curso);
        await _context.SaveChangesAsync();
        await _cursoCacheService.InvalidarCursosActivosAsync();
        TempData["Success"] = "Curso actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso is null) return NotFound();

        curso.Activo = !curso.Activo;
        await _context.SaveChangesAsync();
        await _cursoCacheService.InvalidarCursosActivosAsync();
        TempData["Success"] = curso.Activo ? "Curso activado." : "Curso desactivado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> MatriculasPorCurso(int id)
    {
        var curso = await _context.Cursos
            .Include(c => c.Matriculas)
            .ThenInclude(m => m.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);

        return curso is null ? NotFound() : View(curso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoMatricula(int matriculaId, string estado, int cursoId)
    {
        var result = await _matriculaService.CambiarEstadoAsync(matriculaId, estado);
        TempData[result.ok ? "Success" : "Error"] = result.mensaje;
        return RedirectToAction(nameof(MatriculasPorCurso), new { id = cursoId });
    }

    public IActionResult AccessDeniedInfo() => View();
}
