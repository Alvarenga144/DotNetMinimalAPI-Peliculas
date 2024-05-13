using Microsoft.EntityFrameworkCore;

namespace MinimalAPIPeliculas
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }
    }
}
