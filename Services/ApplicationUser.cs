using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Propiedad para almacenar el secreto de 2FA
    public string TwoFactorSecret { get; set; }

    // Propiedad para saber si el 2FA está habilitado
    public bool IsTwoFactorEnabled { get; set; }

    // No es necesario agregar una propiedad para la contraseña
    // La propiedad 'PasswordHash' ya está gestionada por Identity

    // Método para cambiar la contraseña
    public async Task<IdentityResult> ChangePasswordAsync(UserManager<ApplicationUser> userManager, string currentPassword, string newPassword)
    {
        var user = await userManager.FindByIdAsync(this.Id); // Encuentra al usuario por ID
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
        }

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword); // Cambiar la contraseña
        return result;
    }
}
