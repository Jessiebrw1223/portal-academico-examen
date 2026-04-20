using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademicoExamen.Data;
using PortalAcademicoExamen.Models;

namespace PortalAcademicoExamen.Services;

public class CursoCacheService : ICursoCacheService
{
    private const string CacheKey = "cursos_activos";
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;

    public CursoCacheService(ApplicationDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<Curso>> GetCursosActivosAsync()
    {
        var cachedJson = await _cache.GetStringAsync(CacheKey);

        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            var cachedCursos = JsonSerializer.Deserialize<List<Curso>>(cachedJson);
            if (cachedCursos != null)
                return cachedCursos;
        }

        var cursos = await _context.Cursos
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        };

        var json = JsonSerializer.Serialize(cursos);
        await _cache.SetStringAsync(CacheKey, json, options);

        return cursos;
    }

    public async Task InvalidarCursosActivosAsync()
    {
        await _cache.RemoveAsync(CacheKey);
    }
}