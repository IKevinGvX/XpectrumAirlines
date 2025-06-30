using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class GestionPagosModel : PageModel
    {
        public string NombreCliente { get; set; }
        public string CodigoVuelo { get; set; }
        public decimal TotalPago { get; set; }
        public decimal Descuento { get; set; }
        public string Categoria { get; set; }
        public int ReservaId { get; set; }

        public string TipoPago { get; set; }
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public async Task<IActionResult> OnGetAsync(int usuarioId, string codigoVuelo)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
            SELECT TOP 1 r.reservaId, r.totalPago, r.descuento, r.categoria, r.tipoPago, u.nombre
            FROM Reservas r
            INNER JOIN Usuarios u ON u.usuarioId = r.usuarioId
            INNER JOIN Vuelos v ON v.vueloId = r.vueloId
            WHERE u.usuarioId = @usuarioId AND v.CodigoVuelo = @codigoVuelo
            ORDER BY r.fechaReserva DESC";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@codigoVuelo", codigoVuelo);

                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    ReservaId = reader.GetInt32(0); // 👈 Esto es lo importante
                    TotalPago = reader.GetDecimal(1);
                    Descuento = reader.GetDecimal(2);
                    Categoria = reader.GetString(3);
                    TipoPago = reader.GetString(4);
                    NombreCliente = reader.GetString(5);
                    CodigoVuelo = codigoVuelo;
                }
            }

            return Page();
        }
    }
}
