using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using ZXing;

namespace Xpectrum_Structure.Pages.MisViajes
{
    [Authorize]
    public class ReservasAdicionalesModel : PageModel
    {
        public class VueloDisponible
        {
            public int VueloId { get; set; }
            public string CodigoVuelo { get; set; }
            public string Aerolinea { get; set; }
            public DateTime FechaSalida { get; set; }
            public DateTime FechaLlegada { get; set; }
            public decimal PrecioPEN { get; set; }
            public decimal PrecioUSD { get; set; }
        }


        public List<VueloDisponible> VuelosDisponibles { get; set; } = new();
        public string NombreUsuario { get; set; } = "";
        public string EmailUsuario { get; set; } = "";
        public string Mensaje { get; set; }

        private readonly string connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public void OnGet()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                Mensaje = "No autenticado.";
                return;
            }

            using SqlConnection conn = new(connectionString);
            conn.Open();

            // Obtener datos del usuario autenticado
            using (SqlCommand cmdUsuario = new SqlCommand("SELECT nombre, email FROM Usuarios WHERE usuarioId = @usuarioId", conn))
            {
                cmdUsuario.Parameters.AddWithValue("@usuarioId", usuarioId);
                using SqlDataReader reader = cmdUsuario.ExecuteReader();
                if (reader.Read())
                {
                    NombreUsuario = reader["nombre"].ToString();
                    EmailUsuario = reader["email"].ToString();
                }
            }

            // Buscar vuelos no reservados aún
            string query = @"
    SELECT v.vueloId, v.codigoVuelo, v.aerolinea, v.fechaSalida, v.fechaLlegada, v.precioPEN, v.precioUSD
    FROM Vuelos v
    WHERE v.vueloId NOT IN (
        SELECT r.vueloId FROM Reservas r WHERE r.usuarioId = @usuarioId
    )";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

            using SqlDataReader reader2 = cmd.ExecuteReader();
            while (reader2.Read())
            {
                VuelosDisponibles.Add(new VueloDisponible
                {
                    VueloId = reader2.GetInt32(0),
                    CodigoVuelo = reader2["codigoVuelo"].ToString(),
                    Aerolinea = reader2["aerolinea"].ToString(),
                    FechaSalida = reader2.GetDateTime(reader2.GetOrdinal("fechaSalida")),
                    FechaLlegada = reader2.GetDateTime(reader2.GetOrdinal("fechaLlegada")),
                    PrecioPEN = reader2.GetDecimal(reader2.GetOrdinal("precioPEN")),
                    PrecioUSD = reader2.GetDecimal(reader2.GetOrdinal("precioUSD"))
                });

            }

            if (VuelosDisponibles.Count == 0)
            {
                Mensaje = "No hay vuelos adicionales disponibles para reservar.";
            }
        }

        public void OnPostReservar(int vueloId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                Mensaje = "No autenticado.";
                return;
            }

            using SqlConnection conn = new(connectionString);
            conn.Open();

            string insert = @"
                INSERT INTO Reservas (usuarioId, vueloId, fechaReserva, estadoReserva, tipoPago, totalPago, categoria)
                VALUES (@usuarioId, @vueloId, GETDATE(), 'Confirmada', 'Yape', 0, 'Económica')";

            using SqlCommand cmd = new(insert, conn);
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
            cmd.Parameters.AddWithValue("@vueloId", vueloId);

            cmd.ExecuteNonQuery();
        }
    }
}
