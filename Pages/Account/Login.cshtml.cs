using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
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
                    new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
                    new Claim(ClaimTypes.Role, userInfo.TipoUsuario)  // Aquí cambiamos "Role" por "TipoUsuario"
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Iniciar sesión
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Actualizar la columna fechaUltimoLogin en la base de datos
                UpdateLastLogin(userInfo.UserId);

                // Redirigir según el tipo de usuario
                if (userInfo.TipoUsuario == "Administrador")
                {
                    return RedirectToPage("/Administrador/Index");
                }
                else if (userInfo.TipoUsuario == "Cliente")
                {
                    return RedirectToPage("/Index");
                }
                else if (userInfo.TipoUsuario == "Agente de vuelos")
                {
                    return RedirectToPage("/AgenteDeVuelos/GestionVuelos");
                }
                else if (userInfo.TipoUsuario == "Atención al cliente")
                {
                    return RedirectToPage("/AtencionAlCliente/ConsultaCliente");
                }
                else if (userInfo.TipoUsuario == "Contabilidad")
                {
                    return RedirectToPage("/Contabilidad/GestionPagos");
                }
                else if (userInfo.TipoUsuario == "Supervisor de operaciones")
                {
                    return RedirectToPage("/Operaciones/SupervisionOperaciones");
                }
                else if (userInfo.TipoUsuario == "Gerente de ventas")
                {
                    return RedirectToPage("/Ventas/VentaBoletos");
                }
                else if (userInfo.TipoUsuario == "Jefe de tripulación")
                {
                    return RedirectToPage("/Tripulacion/AsignarTripulacion");
                }
                else if (userInfo.TipoUsuario == "Encargado de equipaje")
                {
                    return RedirectToPage("/Equipaje/GestionEquipaje");
                }
                else
                {
                    // Si el tipo de usuario es "Void" o no está definido, mostrar error
                    TempData["ErrorMessage"] = "Acción no permitida. Contacte con soporte.";
                    return RedirectToPage("/Erronea/PaginaErrorVoid");
                }
            }
            else
            {
                // Mostrar error si las credenciales son incorrectas
                TempData["ErrorMessage"] = result;
                return Page();
            }
        }

        // Método para obtener la información del usuario desde la base de datos
        public (string name, int UserId, string TipoUsuario) GetUserInfoFromDb(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT nombre, usuarioid, tipoUsuario FROM usuarios WHERE Email = @Email";
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
                            var tipoUsuario = reader["tipoUsuario"].ToString(); // Cambiamos "rol" por "tipoUsuario"
                            return (name, userId, tipoUsuario);
                        }
                        else
                        {
                            return (null, 0, null);  // Si no se encuentra el usuario, devolver null
                        }
                    }
                }
            }
        }

        // Validar las credenciales del usuario
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

        // Método para actualizar la fecha de último login
        public void UpdateLastLogin(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE usuarios SET fechaUltimoLogin = GETDATE() WHERE usuarioId = @UsuarioId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
