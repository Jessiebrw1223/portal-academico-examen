using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        const string coordinatorRole = "Coordinador";
        const string coordinatorEmail = "coordinador@portal.local";
        const string coordinatorPassword = "Coord123";

        if (!await roleManager.RoleExistsAsync(coordinatorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(coordinatorRole));
        }

        var coordinator = await userManager.FindByEmailAsync(coordinatorEmail);
        if (coordinator is null)
        {
            coordinator = new IdentityUser
            {
                UserName = coordinatorEmail,
                Email = coordinatorEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(coordinator, coordinatorPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(coordinator, coordinatorRole);
            }
        }
        else if (!await userManager.IsInRoleAsync(coordinator, coordinatorRole))
        {
            await userManager.AddToRoleAsync(coordinator, coordinatorRole);
        }

        if (!await context.Cursos.AnyAsync())
        {
            context.Cursos.AddRange(
                new Curso
                {
                    Codigo = "INF101",
                    Nombre = "Fundamentos de Programación",
                    Creditos = 4,
                    CupoMaximo = 25,
                    HorarioInicio = new TimeSpan(8, 0, 0),
                    HorarioFin = new TimeSpan(10, 0, 0),
                    Activo = true
                },
                new Curso
                {
                    Codigo = "BD201",
                    Nombre = "Bases de Datos I",
                    Creditos = 3,
                    CupoMaximo = 30,
                    HorarioInicio = new TimeSpan(10, 30, 0),
                    HorarioFin = new TimeSpan(12, 0, 0),
                    Activo = true
                },
                new Curso
                {
                    Codigo = "WEB301",
                    Nombre = "Desarrollo Web con ASP.NET Core",
                    Creditos = 5,
                    CupoMaximo = 20,
                    HorarioInicio = new TimeSpan(14, 0, 0),
                    HorarioFin = new TimeSpan(16, 0, 0),
                    Activo = true
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
