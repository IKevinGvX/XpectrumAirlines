using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Xpectrum_Structure.Pages.Profile
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

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


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
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

                        // Asignando "Vois" al campo tipoUsuario
                        command.Parameters.AddWithValue("@tipoUsuario", "Void"); // Asigna "Cliente" como tipo de usuario

                        // Otros campos
                        command.Parameters.AddWithValue("@activo", true); // El usuario está activo por defecto
                        command.Parameters.AddWithValue("@fechaRegistro", DateTime.Now); // Fecha actual
                        command.Parameters.AddWithValue("@estado", "Activo"); // Estado por defecto
                        command.Parameters.AddWithValue("@preferenciasNotificaciones", true); // Notificaciones habilitadas por defecto

                        await command.ExecuteNonQueryAsync();
                    }

                }

                Message = "Registration successful! Please log in.";
                IsSuccess = true;
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                Message = "An error occurred while registering. Please try again.";
                IsSuccess = false;
                return Page();
            }
        }

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
