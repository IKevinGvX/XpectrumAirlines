using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class GestionVuelosModel : PageModel
    {
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public List<Vuelo> Vuelos { get; set; } = new();

        public class Vuelo
        {
            public int VueloId { get; set; }
            public string CodigoVuelo { get; set; }
            public DateTime FechaSalida { get; set; }
            public TimeSpan HoraSalida { get; set; }
            public DateTime FechaLlegada { get; set; }
            public TimeSpan HoraLlegada { get; set; }
            public string EstadoVuelo { get; set; }
            public int AeronaveId { get; set; }
            public string Aerolinea { get; set; }
            public string CodigoAerolinea { get; set; }
            public string TripulacionCapitan { get; set; }
            public string TripulacionCopiloto { get; set; }
            public string TripulacionAuxiliares { get; set; }
            public string? EscalaId { get; set; }
            public string? DuracionEscala { get; set; }
            public string? ServiciosAdicionales { get; set; }
            public decimal TarifaEspecial { get; set; }
            public string Moneda { get; set; }
            public string? CondicionesAdicionales { get; set; }
            public decimal PrecioUSD { get; set; }
            public decimal PrecioPEN { get; set; }
            public string? Imagen { get; set; }

            // Campos extendidos de Aeropuertos
            public string OrigenNombre { get; set; }
            public string OrigenCiudad { get; set; }
            public string OrigenPais { get; set; }
            public string OrigenCodigo { get; set; }

            public string DestinoNombre { get; set; }
            public string DestinoCiudad { get; set; }
            public string DestinoPais { get; set; }
            public string DestinoCodigo { get; set; }
        }

        public void OnGet()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var query = @"
                SELECT 
                    v.vueloId, v.codigoVuelo, v.fechaSalida, v.horaSalida, v.fechaLlegada, v.horaLlegada,
                    v.estadoVuelo, v.aeronaveId, v.aerolinea, v.codigoAerolinea,
                    v.tripulacionCapitan, v.tripulacionCopiloto, v.tripulacionAuxiliares,
                    v.escalaId, v.duracionEscala, v.serviciosAdicionales, v.tarifaEspecial, v.moneda,
                    v.condicionesAdicionales, v.precioUSD, v.precioPEN, v.imagen,
                    o.nombre AS OrigenNombre, o.ciudad AS OrigenCiudad, o.pais AS OrigenPais, o.codigoIata AS OrigenCodigo,
                    d.nombre AS DestinoNombre, d.ciudad AS DestinoCiudad, d.pais AS DestinoPais, d.codigoIata AS DestinoCodigo
                FROM Vuelos v
                JOIN Aeropuertos o ON v.origenId = o.aeropuertoId
                JOIN Aeropuertos d ON v.destinoId = d.aeropuertoId";

            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    var vuelo = new Vuelo
                    {
                        VueloId = reader.GetInt32(0),
                        CodigoVuelo = reader.GetString(1),
                        FechaSalida = reader.GetDateTime(2),
                        HoraSalida = reader.GetTimeSpan(3),
                        FechaLlegada = reader.GetDateTime(4),
                        HoraLlegada = reader.GetTimeSpan(5),
                        EstadoVuelo = reader.GetString(6),
                        AeronaveId = reader.GetInt32(7),
                        Aerolinea = reader.GetString(8),
                        CodigoAerolinea = reader.GetString(9),
                        TripulacionCapitan = reader.GetString(10),
                        TripulacionCopiloto = reader.GetString(11),
                        TripulacionAuxiliares = reader.GetString(12),
                        EscalaId = reader.IsDBNull(13) ? null : reader.GetValue(13).ToString(),
                        DuracionEscala = reader.IsDBNull(14) ? null : reader.GetValue(14).ToString(),
                        ServiciosAdicionales = reader.IsDBNull(15) ? null : reader.GetValue(15).ToString(),
                        TarifaEspecial = reader.GetDecimal(16),
                        Moneda = reader.GetString(17),
                        CondicionesAdicionales = reader.IsDBNull(18) ? null : reader.GetValue(18).ToString(),
                        PrecioUSD = reader.GetDecimal(19),
                        PrecioPEN = reader.GetDecimal(20),
                        Imagen = reader.IsDBNull(21) ? null : reader.GetValue(21).ToString(),

                        OrigenNombre = reader.GetValue(22).ToString(),
                        OrigenCiudad = reader.GetValue(23).ToString(),
                        OrigenPais = reader.GetValue(24).ToString(),
                        OrigenCodigo = reader.GetValue(25).ToString(),

                        DestinoNombre = reader.GetValue(26).ToString(),
                        DestinoCiudad = reader.GetValue(27).ToString(),
                        DestinoPais = reader.GetValue(28).ToString(),
                        DestinoCodigo = reader.GetValue(29).ToString()
                    };

                    Vuelos.Add(vuelo);
                }
                catch (InvalidCastException ex)
                {
                    throw new Exception($"❌ Error al leer una columna: {ex.Message}. Posible error en índice del SqlDataReader.");
                }
            }
        }
    }
}