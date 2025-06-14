using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using BCrypt.Net; // Asegúrate de agregar esta librería

namespace Xpectrum_Structure.Pages.Profile
{
    public class ConfiguracionModel : PageModel
    {
        private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        // Acción para manejar el cambio de contraseña
        public IActionResult OnPostChangePassword()
        {
            // Validar que la nueva contraseña y la confirmación sean iguales
            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Las contraseñas no coinciden.";
                return RedirectToPage();
            }

            // Suponemos que tenemos el usuarioId del usuario actual (esto generalmente vendría de la sesión o un token)
            int userId = 1; // Reemplaza esto con la lógica real para obtener el usuario logueado

            // Hashear la nueva contraseña antes de almacenarla
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            // Conexión a la base de datos y ejecución del procedimiento almacenado
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Ejecutar el procedimiento almacenado para cambiar la contraseña
                var parameters = new { UsuarioId = userId, NewPassword = hashedPassword };
                db.Execute("ChangePassword", parameters, commandType: CommandType.StoredProcedure);
            }

            TempData["SuccessMessage"] = "Contraseña actualizada exitosamente.";
            return RedirectToPage();
        }
    }
}