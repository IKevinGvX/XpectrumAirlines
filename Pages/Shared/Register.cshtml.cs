using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Xpectrum_Structure.Pages.Shared
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        // Definir la cadena de conexión para toda la clase
        private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        [TempData]
        public string Message { get; set; }

        [TempData]
        public bool IsSuccess { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100)]
            public string Nombre { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Contra { get; set; }

            [Required]
            [Phone]
            public string Telefono { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime FechaNacimiento { get; set; }

            [Required]
            [StringLength(255)]
            public string Direccion { get; set; }
        }

        // Método para procesar el registro cuando se envía el formulario
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Usar la cadena de conexión de la clase
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // La consulta SQL para insertar el nuevo usuario
                    string query = "INSERT INTO Usuarios (Nombre, Email, contra, Telefono, FechaNacimiento, Direccion, tipoUsuario, activo, fechaRegistro, estado, preferenciasNotificaciones) " +
                                   "VALUES (@Nombre, @Email, @contra, @Telefono, @FechaNacimiento, @Direccion, @tipoUsuario, @activo, @fechaRegistro, @estado, @preferenciasNotificaciones)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", Input.Nombre);
                        command.Parameters.AddWithValue("@Email", Input.Email);
                        command.Parameters.AddWithValue("@contra", HashPassword(Input.Contra));
                        command.Parameters.AddWithValue("@Telefono", Input.Telefono);
                        command.Parameters.AddWithValue("@FechaNacimiento", Input.FechaNacimiento);
                        command.Parameters.AddWithValue("@Direccion", Input.Direccion);

                        // Asignando "Void" al campo tipoUsuario
                        command.Parameters.AddWithValue("@tipoUsuario", "Void");

                        // Otros campos
                        command.Parameters.AddWithValue("@activo", true); // El usuario está activo por defecto
                        command.Parameters.AddWithValue("@fechaRegistro", DateTime.Now); // Fecha actual
                        command.Parameters.AddWithValue("@estado", "Activo"); // Estado por defecto
                        command.Parameters.AddWithValue("@preferenciasNotificaciones", true); // Notificaciones habilitadas por defecto

                        // Ejecutar la consulta
                        await command.ExecuteNonQueryAsync();
                    }

                }

                // Si todo salió bien, mostrar mensaje de éxito y redirigir
                Message = "Registration successful! Please log in.";
                IsSuccess = true;
                return RedirectToPage("/Profiles/Welcome");
            }
            catch (Exception ex)
            {
                // Si ocurre un error, mostrar mensaje de error
                Message = "An error occurred while registering. Please try again.";
                IsSuccess = false;
                return Page();
            }
        }

        // Método para hashear la contraseña usando SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
