using System.ComponentModel.DataAnnotations;

public class Usuarios
{
    [Key]
    public int UsuarioId { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Contra { get; set; }
    public string TipoUsuario { get; set; }
    public string Telefono { get; set; }
    public string Direccion { get; set; }

    public DateTime? FechaNacimiento { get; set; }  // Cambiado a nullable
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; }
    public bool PreferenciasNotificaciones { get; set; }
    public string TwoFactorSecret { get; set; }
    public bool IsTwoFactorEnabled { get; set; }

    public string SecretKey { get; set; }
    public DateTime? FechaUltimoLogin { get; set; }  // Cambiado a nullable
}
