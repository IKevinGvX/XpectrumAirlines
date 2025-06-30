using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Xpectrum_Structure.Pages.AtencionAlCliente
{
    public class IndexModel : PageModel
    {
        public int TotalReservas { get; set; }
        public int UsuariosActivos { get; set; }
        public int TicketsPendientes { get; set; }

        public List<ReservaDTO> UltimasReservas { get; set; } = new();
        public List<TicketDTO> UltimosTickets { get; set; } = new();

        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public void OnGet()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Total Reservas
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Reservas", conn))
                {
                    TotalReservas = (int)cmd.ExecuteScalar();
                }

                // Usuarios Activos
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Usuarios WHERE activo = 1", conn))
                {
                    UsuariosActivos = (int)cmd.ExecuteScalar();
                }

                // Tickets Pendientes
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE estadoTicket = 'Pendiente'", conn))
                {
                    TicketsPendientes = (int)cmd.ExecuteScalar();
                }

                // Últimas Reservas
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 5 r.reservaId, v.codigoVuelo, u.nombre, r.estadoReserva
                    FROM Reservas r
                    JOIN Usuarios u ON r.usuarioId = u.usuarioId
                    JOIN Vuelos v ON r.vueloId = v.vueloId
                    ORDER BY r.fechaReserva DESC", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UltimasReservas.Add(new ReservaDTO
                        {
                            ReservaId = reader.GetInt32(0),
                            CodigoVuelo = reader.GetString(1),
                            NombreUsuario = reader.GetString(2),
                            EstadoReserva = reader.GetString(3)
                        });
                    }
                }

                // Últimos Tickets
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 5 t.ticketId, t.asunto, u.nombre, t.estadoTicket
                    FROM Tickets t
                    JOIN Usuarios u ON t.usuarioId = u.usuarioId
                    ORDER BY t.fechaCreacion DESC", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UltimosTickets.Add(new TicketDTO
                        {
                            TicketId = reader.GetInt32(0),
                            Asunto = reader.GetString(1),
                            NombreUsuario = reader.GetString(2),
                            EstadoTicket = reader.GetString(3)
                        });
                    }
                }
            }
        }

        public class ReservaDTO
        {
            public int ReservaId { get; set; }
            public string CodigoVuelo { get; set; }
            public string NombreUsuario { get; set; }
            public string EstadoReserva { get; set; }
        }

        public class TicketDTO
        {
            public int TicketId { get; set; }
            public string Asunto { get; set; }
            public string NombreUsuario { get; set; }
            public string EstadoTicket { get; set; }
        }
    }
}