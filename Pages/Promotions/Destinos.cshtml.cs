using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Xpectrum_Structure.Pages.Promotions
{
    public class DestinosModel : PageModel
    {
        public List<DestinoPromo> Destinos { get; set; } = new();

        private readonly string connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public void OnGet()
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                SELECT 
                    v.codigoVuelo,
                    ao.ciudad AS ciudadOrigen,
                    ao.pais AS paisOrigen,
                    ad.ciudad AS ciudadDestino,
                    ad.pais AS paisDestino,
                    v.fechaSalida,
                    v.horaSalida,
                    v.fechaLlegada,
                    v.horaLlegada,
                    v.estadoVuelo,
                    v.aerolinea,
                    v.precioUSD,
                    v.moneda
                FROM Vuelos v
                INNER JOIN Aeropuertos ao ON v.origenId = ao.aeropuertoId
                INNER JOIN Aeropuertos ad ON v.destinoId = ad.aeropuertoId
                WHERE 
                    MONTH(v.fechaSalida) = MONTH(GETDATE()) AND
                    YEAR(v.fechaSalida) = YEAR(GETDATE())
                ORDER BY v.fechaSalida DESC;";

            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Destinos.Add(new DestinoPromo
                {
                    CodigoVuelo = reader["codigoVuelo"].ToString(),
                    CiudadOrigen = reader["ciudadOrigen"].ToString(),
                    PaisOrigen = reader["paisOrigen"].ToString(),
                    CiudadDestino = reader["ciudadDestino"].ToString(),
                    PaisDestino = reader["paisDestino"].ToString(),
                    FechaSalida = ((DateTime)reader["fechaSalida"]).ToShortDateString(),
                    HoraSalida = reader["horaSalida"].ToString(),
                    Aerolinea = reader["aerolinea"].ToString(),
                    Precio = (decimal)reader["precioUSD"],
                    Moneda = reader["moneda"].ToString()
                });
            }
        }

        public class DestinoPromo
        {
            public string CodigoVuelo { get; set; }
            public string CiudadOrigen { get; set; }
            public string PaisOrigen { get; set; }
            public string CiudadDestino { get; set; }
            public string PaisDestino { get; set; }
            public string FechaSalida { get; set; }
            public string HoraSalida { get; set; }
            public string Aerolinea { get; set; }
            public decimal Precio { get; set; }
            public string Moneda { get; set; }
        }
    }
}

