using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Xpectrum_Structure.Pages.Administrador
{
    public class GestionUsuariosModel : PageModel
    {
        private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        public List<UsuarioItem> Usuarios { get; set; } = new List<UsuarioItem>();

        public void OnGet()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"SELECT usuarioId, nombre, email, tipoUsuario, telefono, direccion, fechaNacimiento, activo, estado, preferenciasNotificaciones 
                              FROM usuarios";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Usuarios.Add(new UsuarioItem
                        {
                            UsuarioId = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Email = reader.GetString(2),
                            TipoUsuario = reader.GetString(3),
                            Telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Direccion = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            FechaNacimiento = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6),
                            Activo = reader.GetBoolean(7),
                            Estado = reader.IsDBNull(8) ? "desconocido" : reader.GetString(8),
                            PreferenciasNotificaciones = reader.IsDBNull(9) ? "Ninguna" : reader.GetString(9)
                        });
                    }
                }
            }
        }

        public IActionResult OnGetEliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM usuarios WHERE usuarioId = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] = "Usuario eliminado correctamente.";
            return RedirectToPage("/Administrador/GestionUsuarios");
        }

        public class UsuarioItem
        {
            public int UsuarioId { get; set; }
            public string Nombre { get; set; }
            public string Email { get; set; }
            public string TipoUsuario { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public bool Activo { get; set; }
            public string Estado { get; set; }
            public string PreferenciasNotificaciones { get; set; }
        }
    }
}
