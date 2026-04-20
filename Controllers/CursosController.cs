using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoExamen.Data;
using PortalAcademicoExamen.Services;
using PortalAcademicoExamen.ViewModels;

namespace PortalAcademicoExamen.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICursoCacheService _cursoCacheService;

    public CursosController(ApplicationDbContext context, ICursoCacheService cursoCacheService)
    {
        _context = context;
        _cursoCacheService = cursoCacheService;
    }

    public async Task<IActionResult> Index(CursoCatalogoFiltroViewModel filtro)
    {
        if (filtro.CreditosMin < 0)
            ModelState.AddModelError(nameof(filtro.CreditosMin), "No se aceptan créditos negativos.");
        if (filtro.CreditosMax < 0)
            ModelState.AddModelError(nameof(filtro.CreditosMax), "No se aceptan créditos negativos.");
        if (filtro.HorarioInicio.HasValue && filtro.HorarioFin.HasValue && filtro.HorarioFin < filtro.HorarioInicio)
            ModelState.AddModelError(nameof(filtro.HorarioFin), "No permitir HorarioFin anterior a HorarioInicio.");

        var cursos = await _cursoCacheService.GetCursosActivosAsync();
        var query = cursos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Nombre))
            query = query.Where(c => c.Nombre.Contains(filtro.Nombre, StringComparison.OrdinalIgnoreCase));
        if (filtro.CreditosMin.HasValue)
            query = query.Where(c => c.Creditos >= filtro.CreditosMin.Value);
        if (filtro.CreditosMax.HasValue)
            query = query.Where(c => c.Creditos <= filtro.CreditosMax.Value);
        if (filtro.HorarioInicio.HasValue)
            query = query.Where(c => c.HorarioInicio >= filtro.HorarioInicio.Value);
        if (filtro.HorarioFin.HasValue)
            query = query.Where(c => c.HorarioFin <= filtro.HorarioFin.Value);

        filtro.Cursos = query.OrderBy(c => c.Nombre).ToList();
        return View(filtro);
    }

    public async Task<IActionResult> Details(int id)
    {
        var curso = await _context.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

        if (curso is null)
            return NotFound();

        HttpContext.Session.SetInt32("UltimoCursoId", curso.Id);
        HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

        return View(curso);
    }
}
