using Microsoft.EntityFrameworkCore;
using Xpectrum_Structure.Pages;
using Xpectrum_Structure.Pages.Promotions;

public class VueloService
{
    private readonly ApplicationDbContext _context;
    private readonly VueloService _vueloService;

    public async Task<List<VueloViewModel>> ListarVuelosAsync()
    {
        var vuelos = await _context.Vuelos
            .FromSqlRaw("EXEC ListarVuelos") // Llamada al procedimiento almacenado
            .ToListAsync();

        // Asegúrate de que 'vuelo' sea un objeto que coincida con 'VueloViewModel'
        var vuelosViewModel = vuelos.Select(v => new VueloViewModel
        {
            CodigoVuelo = v.CodigoVuelo,
            AeropuertoOrigen = v.AeropuertoOrigen,
            CiudadOrigen = v.CiudadOrigen,
            FechaSalida = v.FechaSalida,
            FechaLlegada = v.FechaLlegada,
            EstadoVuelo = v.EstadoVuelo,
            // Asegúrate de mapear los campos correctos
        }).ToList();

        return vuelosViewModel;
    }
    public class Vuelos
    {
        public string CodigoVuelo { get; set; }
        public DateTime? FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public DateTime? FechaLlegada { get; set; }
        public string HoraLlegada { get; set; }
        public string EstadoVuelo { get; set; }
        public string Aerolinea { get; set; }
        public double PrecioUSD { get; set; }
        public double PrecioPEN { get; set; }

        // Nuevas propiedades para los datos del aeropuerto de origen
        public string AeropuertoOrigen { get; set; }
        public string CiudadOrigen { get; set; }
        public string PaisOrigen { get; set; }
    }
}
