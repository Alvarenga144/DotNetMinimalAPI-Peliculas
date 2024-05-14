using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Entidades;

namespace MinimalAPIPeliculas
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genero>().Property(p => p.Nombre).HasMaxLength(50);
        }

        public DbSet<Genero> Generos { get; set; }
    }
}
