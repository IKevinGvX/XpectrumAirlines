using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Xpectrum_Structure.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Usuarios> Usuarios { get; set; }  // DbSet para la tabla de usuarios

    public DbSet<VueloViewModel> Vuelos { get; set; }
}
