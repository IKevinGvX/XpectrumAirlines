using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Xpectrum_Structure.Pages.MisViajes
{
    [Authorize]
    public class CheckInModel : PageModel
    {
        public class BoletoInfo
        {
            public int BoletoId { get; set; }
            public string CodigoVuelo { get; set; }
            public string Aerolinea { get; set; }
            public DateTime FechaSalida { get; set; }
            public string EstadoCheckIn { get; set; }
        }

        public List<BoletoInfo> BoletosDisponibles { get; set; } = new();
        public string Mensaje { get; set; }
        public string EmailUsuario { get; set; } = "";
        public string NombreUsuario { get; set; } = "";

        private readonly string connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public void OnGet()
        {
            // Obtener usuarioId autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                Mensaje = "Usuario no autenticado.";
                return;
            }

            using SqlConnection conn = new(connectionString);
            conn.Open();

            // Obtener correo y nombre del usuario autenticado
            using (SqlCommand cmdInfo = new SqlCommand("SELECT nombre, email FROM Usuarios WHERE usuarioId = @usuarioId", conn))
            {
                cmdInfo.Parameters.AddWithValue("@usuarioId", usuarioId);
                using SqlDataReader reader = cmdInfo.ExecuteReader();
                if (reader.Read())
                {
                    NombreUsuario = reader["nombre"].ToString();
                    EmailUsuario = reader["email"].ToString();
                }
            }

            // Buscar boletos pendientes de check-in asociados a reservas confirmadas
            string query = @"
                SELECT b.boletoId, v.codigoVuelo, v.aerolinea, v.fechaSalida, c.estadoCheckIn
                FROM Boletos b
                INNER JOIN Reservas r ON b.reservaId = r.reservaId
                INNER JOIN Vuelos v ON r.vueloId = v.vueloId
                LEFT JOIN CheckIns c ON b.boletoId = c.boletoId
                WHERE r.usuarioId = @usuarioId AND r.estadoReserva = 'Confirmada'
                      AND (c.estadoCheckIn IS NULL OR c.estadoCheckIn = 'Pendiente')";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

            using SqlDataReader reader2 = cmd.ExecuteReader();
            while (reader2.Read())
            {
                DateTime fechaSalida = reader2.GetDateTime(reader2.GetOrdinal("fechaSalida"));

                // Solo permitir check-in si el vuelo es dentro de 48h
                if ((fechaSalida - DateTime.Now).TotalHours > 48)
                    continue;

                BoletosDisponibles.Add(new BoletoInfo
                {
                    BoletoId = reader2.GetInt32(reader2.GetOrdinal("boletoId")),
                    CodigoVuelo = reader2["codigoVuelo"].ToString(),
                    Aerolinea = reader2["aerolinea"].ToString(),
                    FechaSalida = fechaSalida,
                    EstadoCheckIn = reader2["estadoCheckIn"]?.ToString() ?? "Pendiente"
                });
            }

            if (BoletosDisponibles.Count == 0)
            {
                Mensaje = "No tienes vuelos habilitados para hacer Check-In.";
            }
        }

        public void OnPostCheckIn(int boletoId)
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            string insert = @"
                INSERT INTO CheckIns (boletoId, fechaCheckIn, metodoCheckIn, tarjetaEmbarque, estadoCheckIn, numEquipaje)
                VALUES (@boletoId, @fecha, @metodo, @tarjeta, @estado, @equipaje)";

            using SqlCommand cmd = new(insert, conn);
            cmd.Parameters.AddWithValue("@boletoId", boletoId);
            cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
            cmd.Parameters.AddWithValue("@metodo", "Web");
            cmd.Parameters.AddWithValue("@tarjeta", "CHK" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper());
            cmd.Parameters.AddWithValue("@estado", "Completado");
            cmd.Parameters.AddWithValue("@equipaje", 1);

            cmd.ExecuteNonQuery();
        }
    }
}
