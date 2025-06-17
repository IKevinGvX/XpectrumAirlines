using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Xpectrum_Structure.Pages.Administrador
{
    public class GeneralesModel : PageModel
    {
        private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Obtener la acci�n del bot�n que se presion�
            string action = Request.Form["action"];

            switch (action)
            {
                case "updateStatus":
                    // Llamamos a la funci�n para actualizar el estado de los usuarios
                    UpdateUserStatus();
                    TempData["Message"] = "Estado de los usuarios actualizado correctamente.";
                    break;
                case "updatePreferences":
                    // Llamamos a la funci�n para actualizar las preferencias de notificaci�n
                    UpdateUserPreferences();
                    TempData["Message"] = "Preferencias de notificaci�n actualizadas correctamente.";
                    break;
                case "auditChanges":
                    // Llamamos a la funci�n para realizar auditor�a de cambios
                    PerformUserAudit();
                    TempData["Message"] = "Auditor�a de cambios realizada correctamente.";
                    break;
                default:
                    TempData["Error"] = "Acci�n no v�lida.";
                    break;
            }

            // Despu�s de realizar la acci�n, redirigimos a la misma p�gina
            return RedirectToPage("/Administrador/Generales");
        }

        private void UpdateUserStatus()
        {
            string query = @"
        UPDATE usuarios
        SET estado = 'Inactivo'
        WHERE DATEDIFF(MONTH, fechaUltimoLogin, GETDATE()) > 6;";  // Actualizar a los usuarios que no han iniciado sesi�n en los �ltimos 6 meses

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el estado de los usuarios", ex);
            }
        }


        // M�todo para actualizar las preferencias de notificaci�n
        private void UpdateUserPreferences()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DECLARE @usuarioId INT, @preferenciasNotificaciones NVARCHAR(255); " +
                               "DECLARE usuarios_cursor CURSOR FOR " +
                               "SELECT usuarioId, preferenciasNotificaciones " +
                               "FROM usuarios; " +
                               "OPEN usuarios_cursor; FETCH NEXT FROM usuarios_cursor INTO @usuarioId, @preferenciasNotificaciones; " +
                               "WHILE @@FETCH_STATUS = 0 BEGIN " +
                               "IF CHARINDEX('email', @preferenciasNotificaciones) = 0 BEGIN " +
                               "SET @preferenciasNotificaciones = @preferenciasNotificaciones + ', email'; " +
                               "UPDATE usuarios SET preferenciasNotificaciones = @preferenciasNotificaciones WHERE usuarioId = @usuarioId; " +
                               "END FETCH NEXT FROM usuarios_cursor INTO @usuarioId, @preferenciasNotificaciones; " +
                               "END CLOSE usuarios_cursor; DEALLOCATE usuarios_cursor;";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // M�todo para realizar auditor�a de cambios
        private void PerformUserAudit()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DECLARE @usuarioId INT, @estado NVARCHAR(50), @fechaUltimoLogin DATETIME, @fechaCambio DATETIME; " +
                               "DECLARE usuarios_cursor CURSOR FOR " +
                               "SELECT usuarioId, estado, fechaUltimoLogin " +
                               "FROM usuarios; " +
                               "OPEN usuarios_cursor; FETCH NEXT FROM usuarios_cursor INTO @usuarioId, @estado, @fechaUltimoLogin; " +
                               "WHILE @@FETCH_STATUS = 0 BEGIN " +
                               "SET @fechaCambio = GETDATE(); " +
                               "INSERT INTO AuditoriaUsuarios (usuarioId, estadoAnterior, fechaUltimoLogin, fechaCambio) " +
                               "VALUES (@usuarioId, @estado, @fechaUltimoLogin, @fechaCambio); " +
                               "FETCH NEXT FROM usuarios_cursor INTO @usuarioId, @estado, @fechaUltimoLogin; " +
                               "END CLOSE usuarios_cursor; DEALLOCATE usuarios_cursor;";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
