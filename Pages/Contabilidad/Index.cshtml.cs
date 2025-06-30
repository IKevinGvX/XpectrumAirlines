using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Xpectrum_Structure.Pages.Contabilidad
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
        public class ContabilidadModel : PageModel
        {
            public decimal TotalVentasMes { get; set; }
            public int UsuariosActivos { get; set; }
            public Dictionary<string, int> TiposUsuario { get; set; }
            public Dictionary<string, int> EstadoUsuarios { get; set; }
            public Dictionary<string, int> CategoriasServicios { get; set; }
            public Dictionary<string, int> ServiciosMasUsados { get; set; }
            public Dictionary<string, int> EstadosTicket { get; set; }
            public Dictionary<string, int> EstadosVuelo { get; set; }

            public void OnGet()
            {
                string connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    TotalVentasMes = ExecuteScalarDecimal(conn, "SELECT SUM(monto) FROM dbo.Pagos WHERE MONTH(fechaPago) = MONTH(GETDATE()) AND YEAR(fechaPago) = YEAR(GETDATE())");
                    UsuariosActivos = ExecuteScalarInt(conn, "SELECT COUNT(*) FROM dbo.Usuarios WHERE activo = 1");

                    TiposUsuario = ExecuteDictionary(conn, "SELECT tipoUsuario, COUNT(*) FROM dbo.Usuarios WHERE tipoUsuario IS NOT NULL GROUP BY tipoUsuario");
                    EstadoUsuarios = ExecuteDictionary(conn, "SELECT CASE WHEN activo = 1 THEN 'Activo' ELSE 'Inactivo' END, COUNT(*) FROM dbo.Usuarios GROUP BY activo");
                    CategoriasServicios = ExecuteDictionary(conn, "SELECT categoria, COUNT(*) FROM dbo.ServiciosAdicionales GROUP BY categoria");
                    ServiciosMasUsados = ExecuteDictionary(conn, "SELECT nombreServicio, COUNT(*) FROM dbo.ServiciosAdicionales GROUP BY nombreServicio");
                    EstadosTicket = ExecuteDictionary(conn, "SELECT estadoTicket, COUNT(*) FROM dbo.Tickets GROUP BY estadoTicket");
                    EstadosVuelo = ExecuteDictionary(conn, "SELECT estadoVuelo, COUNT(*) FROM dbo.Vuelos GROUP BY estadoVuelo");
                }
            }

            private decimal ExecuteScalarDecimal(SqlConnection conn, string query)
            {
                using var cmd = new SqlCommand(query, conn);
                return cmd.ExecuteScalar() != DBNull.Value ? Convert.ToDecimal(cmd.ExecuteScalar()) : 0;
            }

            private int ExecuteScalarInt(SqlConnection conn, string query)
            {
                using var cmd = new SqlCommand(query, conn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }

            private Dictionary<string, int> ExecuteDictionary(SqlConnection conn, string query)
            {
                var result = new Dictionary<string, int>();
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string key = reader[0]?.ToString() ?? "Desconocido";
                    int value = Convert.ToInt32(reader[1]);
                    result[key] = value;
                }
                return result;
            }
        }

    }
}
