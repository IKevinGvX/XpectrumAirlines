    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Data.SqlClient;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System;

    namespace Xpectrum_Structure.Pages.AtencionAlCliente
    {
        public class SoporteModel : PageModel
        {
            [BindProperty]
            public TicketInputModel NuevoTicket { get; set; }

            public List<TicketViewModel> ListaTickets { get; set; }

            public UsuarioViewModel UsuarioActual { get; set; }

            [BindProperty(SupportsGet = true)]
            public int? UsuarioIdSeleccionado { get; set; }

            public List<SelectListItem> UsuariosLista { get; set; }

            [TempData]
            public string MensajeExito { get; set; }

            [TempData]
            public string MensajeError { get; set; }

            private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public void OnGet()
        {
            CargarListaUsuarios();

            Console.WriteLine("UsuarioIdSeleccionado: " + UsuarioIdSeleccionado);

            int usuarioId = UsuarioIdSeleccionado ?? 1;

            UsuarioActual = ObtenerUsuarioPorId(usuarioId);
            ListaTickets = ObtenerUltimosTicketsPorUsuario(usuarioId);
            NuevoTicket = new TicketInputModel();
        }

        public IActionResult OnPost()
            {
                CargarListaUsuarios();

                int usuarioId = UsuarioIdSeleccionado ?? 1;

                if (!ModelState.IsValid)
                {
                    MensajeError = "Por favor, completa todos los campos requeridos.";
                    UsuarioActual = ObtenerUsuarioPorId(usuarioId);
                    ListaTickets = ObtenerUltimosTicketsPorUsuario(usuarioId);
                    return Page();
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Tickets (usuarioId, fechaCreacion, asunto, descripcion, estadoTicket) VALUES (@usuarioId, GETDATE(), @asunto, @descripcion, 'Pendiente')", conn))
                        {
                            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                            cmd.Parameters.AddWithValue("@asunto", NuevoTicket.Asunto);
                            cmd.Parameters.AddWithValue("@descripcion", NuevoTicket.Descripcion);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MensajeExito = "Tu ticket ha sido enviado correctamente.";
                    return RedirectToPage(new { UsuarioIdSeleccionado = usuarioId });
                }
                catch (Exception ex)
                {
                    MensajeError = "Error al enviar el ticket: " + ex.Message;
                    UsuarioActual = ObtenerUsuarioPorId(usuarioId);
                    ListaTickets = ObtenerUltimosTicketsPorUsuario(usuarioId);
                    return Page();
                }
            }

            private void CargarListaUsuarios()
            {
                UsuariosLista = new List<SelectListItem>();
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT usuarioId, nombre FROM Usuarios", conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuariosLista.Add(new SelectListItem
                                {
                                    Value = reader.GetInt32(0).ToString(),
                                    Text = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
                catch { }
            }

            private List<TicketViewModel> ObtenerUltimosTicketsPorUsuario(int usuarioId)
            {
                var lista = new List<TicketViewModel>();
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT TOP 10 ticketId, asunto, estadoTicket, fechaCreacion FROM Tickets WHERE usuarioId = @usuarioId ORDER BY fechaCreacion DESC", conn))
                        {
                            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    lista.Add(new TicketViewModel
                                    {
                                        TicketId = reader.GetInt32(0),
                                        Asunto = reader.GetString(1),
                                        EstadoTicket = reader.GetString(2),
                                        FechaCreacion = reader.GetDateTime(3)
                                    });
                                }
                            }
                        }
                    }
                }
                catch { }
                return lista;
            }

            private UsuarioViewModel ObtenerUsuarioPorId(int id)
            {
                UsuarioViewModel usuario = null;
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT usuarioId, nombre, email, tipoUsuario, telefono, direccion FROM Usuarios WHERE usuarioId = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    usuario = new UsuarioViewModel
                                    {
                                        UsuarioId = reader.GetInt32(0),
                                        Nombre = reader.GetString(1),
                                        Email = reader.GetString(2),
                                        TipoUsuario = reader.GetString(3),
                                        Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                                        Direccion = reader.IsDBNull(5) ? null : reader.GetString(5)
                                    };
                                }
                            }
                        }
                    }
                }
                catch { }
                return usuario;
            }

            public class TicketInputModel
            {
                [Required(ErrorMessage = "El asunto es obligatorio")]
                public string Asunto { get; set; }

                [Required(ErrorMessage = "La descripción es obligatoria")]
                public string Descripcion { get; set; }
            }

            public class TicketViewModel
            {
                public int TicketId { get; set; }
                public string Asunto { get; set; }
                public string EstadoTicket { get; set; }
                public DateTime FechaCreacion { get; set; }
            }

            public class UsuarioViewModel
            {
                public int UsuarioId { get; set; }
                public string Nombre { get; set; }
                public string Email { get; set; }
                public string TipoUsuario { get; set; }
                public string Telefono { get; set; }
                public string Direccion { get; set; }
            }
        }
    }
