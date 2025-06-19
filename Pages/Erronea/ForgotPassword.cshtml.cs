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
                    ModelState.AddModelError(string.Empty, "No se encontró un usuario con ese correo.");
                    StatusMessage = "Error: No se encontró un usuario con ese correo.";
                    return Page();
                }

                // Generar un token de recuperación
                var resetToken = Guid.NewGuid().ToString();
                user.SecretKey = resetToken;
                await _context.SaveChangesAsync();

                // Enviar correo de recuperación
                var correoEnviado = await EnviarCorreoRecuperacion(user.Email, resetToken);

                if (correoEnviado)
                {
                    StatusMessage = "Se ha enviado un enlace de recuperación de contraseña a tu correo.";
                }
                else
                {
                    StatusMessage = "Hubo un error al enviar el correo de recuperación.";
                }

                return RedirectToPage("/Erronea/ResetPasswordConfirmation");
            }
            catch (Exception ex)
            {
                StatusMessage = "Hubo un error al procesar tu solicitud. Por favor, intenta de nuevo más tarde.";
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
                    Subject = "Recuperación de Contraseña",
                    Body = $"Haz clic en el siguiente enlace para restablecer tu contraseña: <a href='{enlaceRecuperacion}'>Restablecer Contraseña</a>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("tuemail@gmail.com", "tucontraseña"),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(mailMessage);
                return true; // Correo enviado con éxito
            }
            catch
            {
                return false; // Si hay un error en el envío del correo
            }
        }
    }

}