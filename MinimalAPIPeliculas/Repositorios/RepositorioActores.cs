using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Entidades;

namespace MinimalAPIPeliculas.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDBContext _dbContext;

        public RepositorioActores(ApplicationDBContext context)
        {
            this._dbContext = context;
        }

        public async Task<List<Actor>> ObtenerTodos()
        {
            return await _dbContext.Actores.OrderBy(a => a.Nombre).ToListAsync();
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
