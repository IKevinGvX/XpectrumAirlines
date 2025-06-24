using Microsoft.EntityFrameworkCore;
using Xpectrum_Structure.Models;

namespace Xpectrum_Structure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vuelo> Vuelos { get; set; }
        public DbSet<Aeronave> Aeronaves { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuraciones adicionales del modelo
            modelBuilder.Entity<Vuelo>(entity =>
            {
                entity.HasKey(e => e.CodigoVuelo);
            });

            modelBuilder.Entity<Aeronave>(entity =>
            {
                entity.HasKey(e => e.AeronaveId);
            });
        }
    }
}
