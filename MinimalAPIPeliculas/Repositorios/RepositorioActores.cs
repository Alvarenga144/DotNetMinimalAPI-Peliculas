using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Utilidades;

namespace MinimalAPIPeliculas.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly HttpContext _HttpContext;

        public RepositorioActores(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._dbContext = context;
            _HttpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = _dbContext.Actores.AsQueryable();
            await _HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(a => a.Nombre).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await _dbContext.Actores.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Actor>> ObtenerPorNombre(string nombre)
        {
            return await _dbContext.Actores
                .Where(a => a.Nombre.Contains(nombre))
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<int> Crear(Actor actor)
        {
            _dbContext.Add(actor);
            await _dbContext.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await _dbContext.Actores.AnyAsync(a => a.Id == id);
        }

        public async Task Actualizar(Actor actor)
        {
            _dbContext.Update(actor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await _dbContext.Actores.Where(a => a.Id == id).ExecuteDeleteAsync();
        }
    }
}
