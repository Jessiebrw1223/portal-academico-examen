using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Curso>(entity =>
        {
            entity.Property(c => c.Codigo)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(150);
        });

        builder.Entity<Curso>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0");
            t.HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
        });

        builder.Entity<Matricula>(entity =>
        {
            entity.Property(m => m.Estado)
                .HasConversion<string>();

            entity.HasOne(m => m.Curso)
                .WithMany(c => c.Matriculas)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}