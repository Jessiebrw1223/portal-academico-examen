using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoExamen.Data;
using PortalAcademicoExamen.Services;
using PortalAcademicoExamen.ViewModels;

namespace PortalAcademicoExamen.Controllers;

[Authorize]
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMatriculaService _matriculaService;

    public MatriculasController(ApplicationDbContext context, IMatriculaService matriculaService)
    {
        _context = context;
        _matriculaService = matriculaService;
    }

    [HttpGet]
    public async Task<IActionResult> Crear(int cursoId)
    {
        var curso = await _context.Cursos.Include(c => c.Matriculas).FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);
        if (curso is null) return NotFound();

        var cuposOcupados = curso.Matriculas.Count(m => m.Estado != Models.EstadoMatricula.Cancelada);
        var vm = new MatricularViewModel
        {
            CursoId = curso.Id,
            CursoNombre = curso.Nombre,
            CursoCodigo = curso.Codigo,
            Creditos = curso.Creditos,
            HorarioInicio = curso.HorarioInicio,
            HorarioFin = curso.HorarioFin,
            CupoDisponible = curso.CupoMaximo - cuposOcupados
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(MatricularViewModel vm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            TempData["Error"] = "Debes iniciar sesión para matricularte.";
            return Redirect("/Identity/Account/Login");
        }

        var result = await _matriculaService.CrearMatriculaPendienteAsync(vm.CursoId, userId);
        vm.Mensaje = result.mensaje;
        vm.Exito = result.ok;

        var curso = await _context.Cursos.Include(c => c.Matriculas).FirstOrDefaultAsync(c => c.Id == vm.CursoId);
        if (curso is not null)
        {
            vm.CursoNombre = curso.Nombre;
            vm.CursoCodigo = curso.Codigo;
            vm.Creditos = curso.Creditos;
            vm.HorarioInicio = curso.HorarioInicio;
            vm.HorarioFin = curso.HorarioFin;
            vm.CupoDisponible = curso.CupoMaximo - curso.Matriculas.Count(m => m.Estado != Models.EstadoMatricula.Cancelada);
        }

        return View(vm);
    }
}
