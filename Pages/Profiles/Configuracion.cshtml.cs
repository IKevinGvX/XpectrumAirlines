using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;

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
        public async Task<IActionResult> OnPostChangePassword()
        {
            // Validar que la nueva contrase�a y la confirmaci�n sean iguales
            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Las contrase�as no coinciden.";
                return RedirectToPage();
            }

            var nombre = User?.Identity?.Name;

            if (string.IsNullOrEmpty(nombre))
            {
                TempData["ErrorMessage"] = "No se ha podido obtener el nombre del usuario.";
                return RedirectToPage("/Account/Login");
            }

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                var user = await db.QuerySingleOrDefaultAsync<ApplicationUser>("SELECT * FROM dbo.Usuarios WHERE nombre = @Nombre", new { Nombre = nombre });

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToPage("/Account/Login");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                var parameters = new { nombre = nombre, NewPassword = hashedPassword };

                await db.ExecuteAsync("UPDATE dbo.Usuarios SET contra = @NewPassword WHERE nombre = @nombre", parameters);

                TempData["SuccessMessage"] = "Contrase�a actualizada exitosamente.";
                return RedirectToPage("");
            }
        }
    }
}