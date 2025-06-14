using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Xpectrum_Structure.Pages.Account
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security.Claims;

    namespace Xpectrum_Structure.Pages.Account
    {

        public class LoginModel : PageModel
        {
            private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

            [BindProperty]
            [Required]
            public string Email { get; set; }

            [BindProperty]
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public string ErrorMessage { get; set; }

            // Método que maneja la petición GET
            public void OnGet()
            {
            }
            public async Task<IActionResult> OnPostLogout()
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);  // Cerrar sesión
                return RedirectToPage("/Index");  // Redirigir a la página de inicio
            }


            public async Task<IActionResult> OnPost()
            {
                var result = ValidateUserLogin(Email, Password);

                if (result == "Inicio de sesión exitoso")
                {
                    // Obtener la información del usuario de la base de datos
                    var userInfo = GetUserInfoFromDb(Email);

                    if (userInfo.name == null)
                    {
                        TempData["ErrorMessage"] = "Usuario no encontrado.";
                        return Page();
                    }

                    // Guardar el nombre del usuario en TempData (para usar en el perfil)
                    TempData["nombre"] = userInfo.name;

                    // Crear las claims para la sesión
                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userInfo.name),
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString())
        };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Iniciar sesión
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirigir a la página de perfil o al destino deseado
                    return RedirectToPage("/Index");
                }// Asegúrate de que la ruta sea la correcta

                else
                {
                    // Mostrar error si las credenciales son incorrectas
                    TempData["ErrorMessage"] = result;
                    return Page();
                }
            }

            public (string name, int UserId) GetUserInfoFromDb(string email)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT nombre, usuarioid FROM usuarios WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var name = reader["nombre"].ToString();
                                var userId = Convert.ToInt32(reader["usuarioid"]);
                                return (name, userId);
                            }
                            else
                            {
                                return (null, 0);  // Si no se encuentra el usuario, devolver null y 0
                            }
                        }
                    }
                }
            }
            public string ValidateUserLogin(string email, string password)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // Consulta para verificar las credenciales del usuario
                    string query = "SELECT nombre, contra FROM usuarios WHERE email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["contra"].ToString();

                                // Comparar la contraseña proporcionada con la almacenada
                                if (password == storedPassword)
                                {
                                    return "Inicio de sesión exitoso";
                                }
                                else
                                {
                                    return "Contraseña incorrecta";
                                }
                            }
                            else
                            {
                                return "Usuario no encontrado";
                            }
                        }
                    }
                }
            }
        }
    }
}