using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Xpectrum_Structure.Pages.Administrador
{
    public class EditarUsuarioModel : PageModel
    {
        private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        [BindProperty]
        public UsuarioItem Usuario { get; set; } = new(); // Evita que sea null

        // Método para cargar los datos del usuario en la vista
        public void OnGet(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Consulta para obtener los datos del usuario
                string queryUser = @"SELECT usuarioId, nombre, email, telefono, direccion, tipoUsuario 
                     FROM usuarios WHERE usuarioId = @id";

                using (SqlCommand cmd = new SqlCommand(queryUser, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Usuario.UsuarioId = reader.GetInt32(0);
                            Usuario.Nombre = reader.GetString(1);
                            Usuario.Email = reader.GetString(2);
                            Usuario.Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            Usuario.Direccion = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            Usuario.TipoUsuario = reader.GetString(5);
                        }
                    }
                }
            }
        }

        // Método para manejar el post de la actualización de usuario
        public IActionResult OnPost()
        {
            // Verificar que el modelo sea válido
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor, complete todos los campos correctamente.";
                return Page();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE usuarios SET 
                nombre = @nombre,
                email = @correo,
                telefono = @telefono,
                direccion = @direccion,
                tipoUsuario = @tipo
                WHERE usuarioId = @id";


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Asignar parámetros correctamente
                        cmd.Parameters.AddWithValue("@id", Usuario.UsuarioId);
                        cmd.Parameters.AddWithValue("@nombre", Usuario.Nombre);
                        cmd.Parameters.AddWithValue("@correo", Usuario.Email);
                        cmd.Parameters.AddWithValue("@telefono", Usuario.Telefono);
                        cmd.Parameters.AddWithValue("@direccion", Usuario.Direccion);
                        cmd.Parameters.AddWithValue("@tipo", Usuario.TipoUsuario);
                        // Ejecutar el comando para actualizar los datos
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            TempData["Error"] = "No se encontraron registros para actualizar con ese ID.";
                            return Page();
                        }
                    }
                }

                TempData["Mensaje"] = "Usuario actualizado correctamente.";
                return RedirectToPage("/Administrador/GestionUsuarios");
            }
            catch (SqlException ex)
            {
                // Manejo de excepciones específico para SQL
                TempData["Error"] = $"Hubo un problema al actualizar el usuario en la base de datos: {ex.Message}";
                return Page();
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                TempData["Error"] = $"Hubo un error inesperado: {ex.Message}";
                return Page();
            }
        }

        // Modelo para los datos del usuario
        public class UsuarioItem
        {
            public int UsuarioId { get; set; }
            public string Nombre { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public string TipoUsuario { get; set; } // NUEVO CAMPO
        }

        public List<string> TiposUsuario { get; } = new List<string>
{
    "Cliente",
    "Administrador",
    "Agente de vuelos",
    "Atención al cliente",
    "Contabilidad",
    "Supervisor de operaciones",
    "Gerente de ventas",
    "Jefe de tripulación",
    "Encargado de equipaje",
    "Usuario"
};

    }
}
