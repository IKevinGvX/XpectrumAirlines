using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xpectrum_Structure.ViewModels;
using XpectrumStructure.Models;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class GestionVuelosModel : PageModel
    {
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public List<Aeropuerto> Aeropuertos { get; set; } = new List<Aeropuerto>();
        public List<Vuelos> VuelosDisponibles { get; set; } = new List<Vuelos>();

        // Método para obtener los aeropuertos
        public async Task OnGetAsync()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            var cmd = new SqlCommand("SELECT AeropuertoId, Nombre, Ciudad, Pais FROM Aeropuertos", con);
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Aeropuertos.Add(new Aeropuerto
                {
                    AeropuertoId = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Ciudad = reader.GetString(2),
                    Pais = reader.GetString(3)
                });
            }
        }

        // Endpoint para obtener los vuelos disponibles según el aeropuerto seleccionado
        public async Task<IActionResult> OnGetReservarAsync(int airportId)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            var cmd = new SqlCommand("SELECT VueloId, CodigoVuelo, Aerolinea, OrigenId, DestinoId, FechaSalida, HoraSalida, FechaLlegada, HoraLlegada FROM Vuelos WHERE OrigenId = @airportId OR DestinoId = @airportId", con);
            cmd.Parameters.AddWithValue("@airportId", airportId);

            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                VuelosDisponibles.Add(new Vuelos
                {
                    VueloId = reader.GetInt32(0),
                    CodigoVuelo = reader.GetString(1),
                    Aerolinea = reader.GetString(2),
                    OrigenId = reader.GetInt32(3),
                    DestinoId = reader.GetInt32(4),
                    FechaSalida = reader.GetDateTime(5),
                    HoraSalida = reader.GetTimeSpan(6),
                    FechaLlegada = reader.GetDateTime(7),
                    HoraLlegada = reader.GetTimeSpan(8)
                });
            }

            return new JsonResult(VuelosDisponibles);
        }
    }
}
