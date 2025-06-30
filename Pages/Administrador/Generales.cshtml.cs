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
            // Obtener la acción del botón que se presionó
            string action = Request.Form["action"];

            switch (action)
            {
                case "updateStatus":
                    // Llamamos a la función para actualizar el estado de los usuarios
                    UpdateUserStatus();
                    TempData["Message"] = "Estado de los usuarios actualizado correctamente.";
                    break;
                case "updatePreferences":
                    // Llamamos a la función para actualizar las preferencias de notificación
                    UpdateUserPreferences();
                    TempData["Message"] = "Preferencias de notificación actualizadas correctamente.";
                    break;
                case "auditChanges":
                    // Llamamos a la función para realizar auditoría de cambios
                    PerformUserAudit();
                    TempData["Message"] = "Auditoría de cambios realizada correctamente.";
                    break;
                default:
                    TempData["Error"] = "Acción no válida.";
                    break;
            }

            // Después de realizar la acción, redirigimos a la misma página
            return RedirectToPage("/Administrador/Generales");
        }

        private void UpdateUserStatus()
        {
            string query = @"
        UPDATE usuarios
        SET estado = 'Inactivo'
        WHERE DATEDIFF(MONTH, fechaUltimoLogin, GETDATE()) > 6;";  // Actualizar a los usuarios que no han iniciado sesión en los últimos 6 meses

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
        public ContentResult OnGetUltimosCambios()
        {
            var html = new System.Text.StringBuilder();

            html.Append("<table style='width:100%; color:#e0e0e0; background-color:#0f172a; border-collapse:collapse; font-size: 0.9rem;'>");
            html.Append("<thead style='background-color:#1e293b;'>");
            html.Append("<tr style='color:#60a5fa; text-align:left;'>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>ID Usuario</th>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>Campo</th>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>Valor Anterior</th>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>Valor Nuevo</th>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>Operación</th>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>Fecha</th>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>Operador</th>");
            html.Append("<th style='padding:8px; border-bottom:1px solid #334155;'>IP</th>");
            html.Append("</tr></thead><tbody>");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT TOP 10 
                usuarioAfectadoId, campoModificado, valorAnterior, valorNuevo, 
                operacion, fechaOperacion, usuarioOperadorId, ipOrigen 
            FROM AuditoriaUsuarios
            ORDER BY fechaOperacion DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int? usuarioId = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                        string campo = reader.IsDBNull(1) ? "N/A" : reader.GetString(1);
                        string valorAnterior = reader.IsDBNull(2) ? "N/A" : reader.GetString(2);
                        string valorNuevo = reader.IsDBNull(3) ? "N/A" : reader.GetString(3);
                        string operacion = reader.IsDBNull(4) ? "N/A" : reader.GetString(4);
                        DateTime? fecha = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
                        int? operadorId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6);
                        string ip = reader.IsDBNull(7) ? "N/A" : reader.GetString(7);

                        html.Append("<tr style='border-bottom:1px solid #334155;'>");
                        html.Append($"<td style='padding:8px'>{(usuarioId.HasValue ? usuarioId.ToString() : "Sin ID")}</td>");
                        html.Append($"<td style='padding:8px'>{campo}</td>");
                        html.Append($"<td style='padding:8px'>{valorAnterior}</td>");
                        html.Append($"<td style='padding:8px'>{valorNuevo}</td>");
                        html.Append($"<td style='padding:8px'>{operacion}</td>");
                        html.Append($"<td style='padding:8px'>{(fecha.HasValue ? fecha.Value.ToString("g") : "Sin fecha")}</td>");
                        html.Append($"<td style='padding:8px'>{(operadorId.HasValue ? operadorId.ToString() : "Desconocido")}</td>");
                        html.Append($"<td style='padding:8px'>{ip}</td>");
                        html.Append("</tr>");
                    }
                }
            }

            html.Append("</tbody></table>");
            return Content(html.ToString(), "text/html");
        }


        // Método para actualizar las preferencias de notificación
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
        private void PerformUserAudit()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
        DECLARE @usuarioId INT, @estadoAnterior NVARCHAR(50), @fechaUltimoLogin DATETIME;
        DECLARE usuarios_cursor CURSOR FOR
        SELECT usuarioId, estado, fechaUltimoLogin FROM usuarios;

        OPEN usuarios_cursor;
        FETCH NEXT FROM usuarios_cursor INTO @usuarioId, @estadoAnterior, @fechaUltimoLogin;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO AuditoriaUsuarios (
                usuarioAfectadoId,
                operacion,
                campoModificado,
                valorAnterior,
                valorNuevo,
                fechaOperacion,
                usuarioOperadorId,
                ipOrigen,
                observaciones
            )
            VALUES (
                @usuarioId,
                'UPDATE',
                'estado',
                @estadoAnterior,
                'AUDITADO',
                GETDATE(),
                1, -- Cambia esto por el ID del admin si lo tienes disponible
                '127.0.0.1', -- O usa HttpContext.Connection.RemoteIpAddress.ToString()
                'Auditoría programada desde Generales.cshtml.cs'
            );

            FETCH NEXT FROM usuarios_cursor INTO @usuarioId, @estadoAnterior, @fechaUltimoLogin;
        END;

        CLOSE usuarios_cursor;
        DEALLOCATE usuarios_cursor;
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
