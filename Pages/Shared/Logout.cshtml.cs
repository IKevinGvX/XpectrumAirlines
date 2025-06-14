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

            return RedirectToPage("/Index"); // Redirigir a la página de inicio
        }

        public async Task<IActionResult> OnGet()
        {
            // Cerrar sesión eliminando las cookies de autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirigir a la página de inicio
            return RedirectToPage("/Index");
        }
    }
}