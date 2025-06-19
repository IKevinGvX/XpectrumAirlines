using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Xpectrum_Structure.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResetPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ResetPasswordInputModel Input { get; set; }

        public string Token { get; set; }

        public class ResetPasswordInputModel
        {
            public string Password { get; set; }
        }

        public void OnGet(string token)
        {
            Token = token;
        }

        public async Task<IActionResult> OnPostAsync(string token)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.SecretKey == token);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Token de recuperaci�n inv�lido.");
                return Page();
            }

            // Actualizar la contrase�a
            user.Contra = Input.Password;  // Aseg�rate de que la contrase�a est� cifrada
            user.SecretKey = null;  // Eliminar el token de recuperaci�n despu�s de usarlo

            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Login");
        }
    }
}