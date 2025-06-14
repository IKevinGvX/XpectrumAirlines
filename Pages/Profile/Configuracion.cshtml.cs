using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using BCrypt.Net; // Aseg�rate de agregar esta librer�a

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

        // Acci�n para manejar el cambio de contrase�a
        public IActionResult OnPostChangePassword()
        {
            // Validar que la nueva contrase�a y la confirmaci�n sean iguales
            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Las contrase�as no coinciden.";
                return RedirectToPage();
            }

            // Suponemos que tenemos el usuarioId del usuario actual (esto generalmente vendr�a de la sesi�n o un token)
            int userId = 1; // Reemplaza esto con la l�gica real para obtener el usuario logueado

            // Hashear la nueva contrase�a antes de almacenarla
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            // Conexi�n a la base de datos y ejecuci�n del procedimiento almacenado
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Ejecutar el procedimiento almacenado para cambiar la contrase�a
                var parameters = new { UsuarioId = userId, NewPassword = hashedPassword };
                db.Execute("ChangePassword", parameters, commandType: CommandType.StoredProcedure);
            }

            TempData["SuccessMessage"] = "Contrase�a actualizada exitosamente.";
            return RedirectToPage();
        }
    }
}