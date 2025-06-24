using Microsoft.EntityFrameworkCore;
using Xpectrum_Structure.Data;
using Xpectrum_Structure.Models;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Services
{
    public class VueloService
    {
        private readonly ApplicationDbContext _context;

        public VueloService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<VueloViewModel>> ListarVuelosAsync()
        {
            try
            {
                var vuelos = await _context.Vuelos
                    .FromSqlRaw("EXEC ListarVuelos") // Llamada al procedimiento almacenado
                    .ToListAsync();

                var vuelosViewModel = vuelos.Select(v => new VueloViewModel
                {
                    CodigoVuelo = v.CodigoVuelo,
                    AeropuertoOrigen = v.AeropuertoOrigen,
                    CiudadOrigen = v.CiudadOrigen,
                    FechaSalida = v.FechaSalida,
                    FechaLlegada = v.FechaLlegada,
                    EstadoVuelo = v.EstadoVuelo,
                    HoraSalida = v.HoraSalida,
                    HoraLlegada = v.HoraLlegada,
                    Aerolinea = v.Aerolinea,
                    PrecioUSD = v.PrecioUSD,
                    PrecioPEN = v.PrecioPEN
                }).ToList();

                return vuelosViewModel;
            }
            catch (Exception)
            {
                // Log the error (consider using ILogger)
                return new List<VueloViewModel>();
            }
        }
    }

    // La clase Vuelo ahora está en Xpectrum_Structure.Models
}
