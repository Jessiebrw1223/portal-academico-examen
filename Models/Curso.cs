using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PortalAcademicoExamen.Models;

[Index(nameof(Codigo), IsUnique = true)]
public class Curso
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, 30)]
    public int Creditos { get; set; }

    [Range(1, 500)]
    public int CupoMaximo { get; set; }

    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}