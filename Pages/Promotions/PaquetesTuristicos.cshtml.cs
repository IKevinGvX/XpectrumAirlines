using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Xpectrum_Structure.Pages.Promotions
{
    public class PaquetesTuristicosModel : PageModel
    {
        public List<PaqueteInfo> Paquetes { get; set; } = new();

        private readonly string connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public void OnGet()
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string query = @"
                SELECT 
                    v.codigoVuelo,
                    ao.ciudad AS ciudadOrigen,
                    ad.ciudad AS ciudadDestino,
                    v.fechaSalida,
                    v.precioUSD,
                    v.serviciosAdicionales
                FROM Vuelos v
                INNER JOIN Aeropuertos ao ON v.origenId = ao.aeropuertoId
                INNER JOIN Aeropuertos ad ON v.destinoId = ad.aeropuertoId
                WHERE MONTH(v.fechaSalida) = MONTH(GETDATE()) 
                AND YEAR(v.fechaSalida) = YEAR(GETDATE())
                AND v.serviciosAdicionales IS NOT NULL";

            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Paquetes.Add(new PaqueteInfo
                {
                    CodigoVuelo = reader["codigoVuelo"].ToString(),
                    Origen = reader["ciudadOrigen"].ToString(),
                    Destino = reader["ciudadDestino"].ToString(),
                    FechaSalida = reader.GetDateTime(reader.GetOrdinal("fechaSalida")).ToShortDateString(),
                    PrecioUSD = reader.GetDecimal(reader.GetOrdinal("precioUSD")),
                    Servicios = reader["serviciosAdicionales"].ToString()
                });
            }
        }

        public class PaqueteInfo
        {
            public string CodigoVuelo { get; set; }
            public string Origen { get; set; }
            public string Destino { get; set; }
            public string FechaSalida { get; set; }
            public decimal PrecioUSD { get; set; }
            public string Servicios { get; set; }
        }
    }
}
