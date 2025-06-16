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
                string connectionString = "Server=DESKTOP-7I9SFFS;Database=Xpectrum;Integrated Security=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Usuarios (Nombre, Email, Contraseña, Telefono, FechaNacimiento, Direccion, RolId) VALUES (@Nombre, @Email, @Contrasena, @Telefono, @FechaNacimiento, @Direccion, 2)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", Input.Nombre);
                        command.Parameters.AddWithValue("@Email", Input.Email);
                        command.Parameters.AddWithValue("@Contrasena", HashPassword(Input.Contra));
                        command.Parameters.AddWithValue("@Telefono", Input.Telefono);
                        command.Parameters.AddWithValue("@FechaNacimiento", Input.FechaNacimiento);
                        command.Parameters.AddWithValue("@Direccion", Input.Direccion);

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
