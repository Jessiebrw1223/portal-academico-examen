using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PortalAcademicoExamen.Models;

[Index(nameof(CursoId), nameof(UsuarioId), IsUnique = true)]
public class Matricula
{
    public int Id { get; set; }

    [Required]
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = default!;

    [Required]
    public string UsuarioId { get; set; } = string.Empty;
    public IdentityUser Usuario { get; set; } = default!;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
}