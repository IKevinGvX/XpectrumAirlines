using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Xpectrum_Structure.Pages.Profile
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public UserRegistrationInput Input { get; set; }

        public class UserRegistrationInput
        {
            [Required]
            [StringLength(150, MinimumLength = 3)]
            public string Nombre { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(255, MinimumLength = 6)]
            public string Contra { get; set; }

            [Required]
            public string Telefono { get; set; }

            [Required]
            public DateTime FechaNacimiento { get; set; }

            [Required]
            [StringLength(255)]
            public string Direccion { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Aquí va el código para conectar a la base de datos y ejecutar el procedimiento almacenado
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("sp_registrar_usuario", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@nombre", Input.Nombre);
                command.Parameters.AddWithValue("@email", Input.Email);
                command.Parameters.AddWithValue("@contra", Input.Contra);
                command.Parameters.AddWithValue("@telefono", Input.Telefono);
                command.Parameters.AddWithValue("@fechaNacimiento", Input.FechaNacimiento);
                command.Parameters.AddWithValue("@direccion", Input.Direccion);

                await command.ExecuteNonQueryAsync();
            }

            return RedirectToPage("/Profile/Welcome");
        }
    }
}