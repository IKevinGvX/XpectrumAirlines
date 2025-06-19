using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Xpectrum_Structure.Pages.Erronea
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ForgotPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ForgotPasswordInputModel Input { get; set; }

        [TempData] // Usamos TempData para pasar mensajes entre acciones
        public string StatusMessage { get; set; }

        public class ForgotPasswordInputModel
        {
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Buscar al usuario por correo
                var user = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == Input.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontr� un usuario con ese correo.");
                    StatusMessage = "Error: No se encontr� un usuario con ese correo.";
                    return Page();
                }

                // Generar un token de recuperaci�n
                var resetToken = Guid.NewGuid().ToString();
                user.SecretKey = resetToken;
                await _context.SaveChangesAsync();

                // Enviar correo de recuperaci�n
                var correoEnviado = await EnviarCorreoRecuperacion(user.Email, resetToken);

                if (correoEnviado)
                {
                    StatusMessage = "Se ha enviado un enlace de recuperaci�n de contrase�a a tu correo.";
                }
                else
                {
                    StatusMessage = "Hubo un error al enviar el correo de recuperaci�n.";
                }

                return RedirectToPage("/Erronea/ResetPasswordConfirmation");
            }
            catch (Exception ex)
            {
                StatusMessage = "Hubo un error al procesar tu solicitud. Por favor, intenta de nuevo m�s tarde.";
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        private async Task<bool> EnviarCorreoRecuperacion(string email, string resetToken)
        {
            try
            {
                var enlaceRecuperacion = Url.Page("/Account/ResetPassword", new { token = resetToken });

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("tuemail@gmail.com"),
                    Subject = "Recuperaci�n de Contrase�a",
                    Body = $"Haz clic en el siguiente enlace para restablecer tu contrase�a: <a href='{enlaceRecuperacion}'>Restablecer Contrase�a</a>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("tuemail@gmail.com", "tucontrase�a"),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(mailMessage);
                return true; // Correo enviado con �xito
            }
            catch
            {
                return false; // Si hay un error en el env�o del correo
            }
        }
    }

}