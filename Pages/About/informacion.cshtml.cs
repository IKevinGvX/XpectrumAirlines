using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Xpectrum_Structure.Pages.About
{
    public class informacionModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "El nombre es requerido")]
        public string? Nombre { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string? Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El mensaje es requerido")]
        public string? Mensaje { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El asunto es requerido")]
        public string? Asunto { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El teléfono es requerido")]
        public string? Telefono { get; set; }

        public string? Message { get; set; }
        public bool ShowMessage { get; set; } = false;

        public IActionResult OnPostContacto()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Aquí iría la lógica para procesar el formulario
            // Por ejemplo, enviar un correo electrónico o guardar en la base de datos

            Message = "¡Gracias por contactarnos! Nos pondremos en contacto contigo pronto.";
            ShowMessage = true;
            
            // Limpiar el formulario
            Nombre = string.Empty;
            Email = string.Empty;
            Mensaje = string.Empty;
            Asunto = string.Empty;
            Telefono = string.Empty;

            return Page();
        }
    }
}
