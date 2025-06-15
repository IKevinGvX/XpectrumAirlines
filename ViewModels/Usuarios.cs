using System;
using System.ComponentModel.DataAnnotations;

namespace Xpectrum_Structure.Models
{
    public class Usuarios
    {
        [Key]
        public int UsuarioId { get; set; }  // ID del usuario (clave primaria)

        [Required]
        [MaxLength(256)]
        public string Nombre { get; set; }  // Nombre del usuario

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }  // Correo electrónico del usuario

        [Required]
        public string Contra { get; set; }  // Contraseña cifrada

        public string TipoUsuario { get; set; }  // Tipo de usuario (admin, cliente, etc.)

        public string Telefono { get; set; }  // Teléfono del usuario (opcional)

        public string Direccion { get; set; }  // Dirección del usuario (opcional)

        public DateTime FechaNacimiento { get; set; }  // Fecha de nacimiento

        public bool Activo { get; set; }  // Estado del usuario (activo o no)

        public DateTime FechaRegistro { get; set; }  // Fecha de registro del usuario

        // Campos relacionados con la autenticación de dos factores (2FA)
        public string TwoFactorSecret { get; set; }  // Clave secreta de 2FA (Base64)

        public bool IsTwoFactorEnabled { get; set; }  // Indica si 2FA está habilitado (true/false)
    }
}
