using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class ConfirmacionPagoModel : PageModel
    {
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        [BindProperty(SupportsGet = true, Name = "TipoPago")]
        public string tipoPago { get; set; }

        [BindProperty(SupportsGet = true, Name = "ReservaId")]
        public int reservaId { get; set; }

        public string MetodoConfirmado { get; set; } // Para mostrar en la vista

        public async Task<IActionResult> OnGetAsync()
        {
            if (reservaId == 0 || string.IsNullOrEmpty(tipoPago))
            {
                return BadRequest("Faltan datos para confirmar el pago.");
            }

            // Actualizar la reserva en la base de datos
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string updateQuery = @"
            UPDATE Reservas
            SET 
                fechaModificacion = @fechaModificacion,
                tipoPago = @tipoPago,
                estadoReserva = @estadoReserva
            WHERE reservaId = @reservaId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@fechaModificacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@tipoPago", tipoPago);
                    cmd.Parameters.AddWithValue("@estadoReserva", "Confirmada");
                    cmd.Parameters.AddWithValue("@reservaId", reservaId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Asignar el método de pago confirmado para mostrar en la vista
            MetodoConfirmado = tipoPago;

            return Page();
        }
    }
}
