using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;

namespace Xpectrum_Structure.Pages.MisViajes
{
    [Authorize]
    public class AdministracionModel : PageModel
    {
        public List<ViajeInfo> MisViajes { get; set; } = new();
        public string Mensaje { get; set; }
        public int UsuarioId { get; set; }
        public string EmailUsuario { get; set; } = "";

        private readonly string connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";
        public void OnGet()
        {
            // Obtener usuarioId desde el Claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                Mensaje = "No se pudo identificar al usuario.";
                return;
            }

            using SqlConnection conn = new(connectionString);
            conn.Open();

            // Obtener correo del usuario desde la base de datos
            using (SqlCommand cmdCorreo = new SqlCommand("SELECT email FROM Usuarios WHERE usuarioId = @usuarioId", conn))
            {
                cmdCorreo.Parameters.AddWithValue("@usuarioId", usuarioId);
                var result = cmdCorreo.ExecuteScalar();
                if (result != null)
                {
                    EmailUsuario = result.ToString();
                }
                else
                {
                    Mensaje = "No se encontró el correo del usuario.";
                    return;
                }
            }

            // Obtener viajes confirmados del usuario
            string query = @"
        SELECT 
            v.codigoVuelo,
            v.aerolinea,
            v.fechaSalida,
            v.horaSalida,
            v.fechaLlegada,
            v.horaLlegada,
            r.fechaReserva,
            r.estadoReserva,
            r.categoria,
            r.tipoPago,
            r.totalPago,
            r.descuento
        FROM Reservas r
        INNER JOIN Vuelos v ON r.vueloId = v.vueloId
        INNER JOIN Usuarios u ON r.usuarioId = u.usuarioId
        WHERE u.usuarioId = @usuarioId AND r.estadoReserva = 'Confirmada'
        ORDER BY v.fechaSalida ASC";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                MisViajes.Add(new ViajeInfo
                {
                    CodigoVuelo = reader["codigoVuelo"].ToString(),
                    Aerolinea = reader["aerolinea"].ToString(),
                    FechaSalida = reader.GetDateTime(reader.GetOrdinal("fechaSalida")).ToShortDateString(),
                    HoraSalida = reader.GetTimeSpan(reader.GetOrdinal("horaSalida")).ToString(@"hh\:mm"),
                    FechaLlegada = reader.GetDateTime(reader.GetOrdinal("fechaLlegada")).ToShortDateString(),
                    HoraLlegada = reader.GetTimeSpan(reader.GetOrdinal("horaLlegada")).ToString(@"hh\:mm"),
                    FechaReserva = reader.GetDateTime(reader.GetOrdinal("fechaReserva")).ToShortDateString(),
                    EstadoReserva = reader["estadoReserva"].ToString(),
                    TipoPago = reader["tipoPago"].ToString(),
                    Categoria = reader["categoria"].ToString(),
                    TotalPago = reader.GetDecimal(reader.GetOrdinal("totalPago")),
                    Descuento = reader.IsDBNull(reader.GetOrdinal("descuento")) ? 0 : reader.GetDecimal(reader.GetOrdinal("descuento"))
                });
            }

            if (MisViajes.Count == 0)
            {
                Mensaje = "No tienes viajes confirmados por el momento.";
            }
        }

        public class ViajeInfo
        {
            public string CodigoVuelo { get; set; }
            public string Aerolinea { get; set; }
            public string FechaSalida { get; set; }
            public string HoraSalida { get; set; }
            public string FechaLlegada { get; set; }
            public string HoraLlegada { get; set; }
            public string FechaReserva { get; set; }
            public string EstadoReserva { get; set; }
            public string TipoPago { get; set; }
            public string Categoria { get; set; }
            public decimal TotalPago { get; set; }
            public decimal Descuento { get; set; }
        }
    }
}

