using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Xpectrum_Structure.Pages.Shared
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar TempData o Session si es necesario
            TempData["nombre"] = null;  // Limpiar el nombre del usuario

            return RedirectToPage("/Index"); // Redirigir a la p�gina de inicio
        }

        public async Task<IActionResult> OnGet()
        {
            // Cerrar sesi�n eliminando las cookies de autenticaci�n
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirigir a la p�gina de inicio
            return RedirectToPage("/Index");
        }
    }
}