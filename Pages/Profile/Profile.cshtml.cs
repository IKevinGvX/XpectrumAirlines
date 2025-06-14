using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static QRCoder.PayloadGenerator;

namespace Xpectrum_Structure.Pages.Profile
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;"; // Conexión a la base de datos
        public Profile UserProfile { get; set; }

        public async Task<IActionResult> OnPostAsync(Profile updatedProfile)
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Si hay errores de validación, regresa a la misma página
            }

            // Actualizar los datos del usuario usando el nombre en lugar del usuarioId
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "UPDATE dbo.Usuarios SET nombre = @Nombre, telefono = @Telefono, direccion = @Direccion, " +
                            "fechaNacimiento = @FechaNacimiento, tipoUsuario = @TipoUsuario, estado = @Estado, " +
                            "preferenciasNotificaciones = @PreferenciasNotificaciones WHERE nombre = @Nombre"; // Usar 'nombre' en lugar de 'usuarioId'

                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", updatedProfile.Nombre); // Usamos el 'Nombre' para identificar al usuario
                cmd.Parameters.AddWithValue("@Telefono", updatedProfile.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", updatedProfile.Direccion);
                cmd.Parameters.AddWithValue("@FechaNacimiento", updatedProfile.FechaNacimiento);
                cmd.Parameters.AddWithValue("@TipoUsuario", updatedProfile.TipoUsuario);
                cmd.Parameters.AddWithValue("@Estado", updatedProfile.Estado);
                cmd.Parameters.AddWithValue("@PreferenciasNotificaciones", updatedProfile.PreferenciasNotificaciones);

                await cmd.ExecuteNonQueryAsync(); // Ejecuta la actualización
            }

            return RedirectToPage(); // Redirigir a la misma página con los datos actualizados
        }


      
        public async Task<IActionResult> OnGetAsync()
        {
            var nombre = User?.Identity?.Name;  // Obtener el email del usuario autenticado

            if (string.IsNullOrEmpty(nombre))
            {
                return RedirectToPage("/Account/Login"); // Si no está autenticado, redirige al login
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Usamos email en lugar de nombre para obtener el perfil
                var query = "SELECT * FROM dbo.Usuarios WHERE nombre = @nombre";
                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombre", nombre);  // Usamos el email del usuario autenticado

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        UserProfile = new Profile
                        {
                            UsuarioId = (int)reader["usuarioId"],
                            Nombre = reader["nombre"]?.ToString(),
                            Email = reader["email"]?.ToString(),
                            Telefono = reader["telefono"]?.ToString(),
                            Direccion = reader["direccion"]?.ToString(),
                            FechaNacimiento = reader["fechaNacimiento"] as DateTime?,
                            TipoUsuario = reader["tipoUsuario"]?.ToString(),
                            Activo = reader["activo"] as bool?,
                            FechaRegistro = reader["fechaRegistro"] as DateTime?,
                            Estado = reader["estado"]?.ToString(),
                            PreferenciasNotificaciones = reader["preferenciasNotificaciones"]?.ToString()
                        };
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No se encontraron los datos del perfil.";
                        return RedirectToPage("/Index"); // Si no se encuentran datos
                    }
                }
            }

            return Page(); // Retorna la página con los datos cargados
        }


        public class Profile
        {

            public int UsuarioId { get; set; } // ID del usuario

            [Required]
            [StringLength(150)]
            public string Nombre { get; set; } // Nombre

            [Required]
            [StringLength(150)]
            public string Email { get; set; } // Email

            [StringLength(20)]
            public string Telefono { get; set; } // Teléfono

            [StringLength(255)]
            public string Direccion { get; set; } // Dirección

            public DateTime? FechaNacimiento { get; set; } // Fecha de Nacimiento

            [StringLength(50)]
            public string TipoUsuario { get; set; } // Tipo de usuario

            public bool? Activo { get; set; } // Estado de la cuenta

            public DateTime? FechaRegistro { get; set; } // Fecha de registro

            [StringLength(20)]
            public string Estado { get; set; } // Estado

            [StringLength(255)]
            public string PreferenciasNotificaciones { get; set; } // Preferencias de notificación
        }

    }

}