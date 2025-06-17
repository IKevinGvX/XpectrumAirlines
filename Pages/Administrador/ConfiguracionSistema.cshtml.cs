using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Xpectrum_Structure.Pages.Administrador
{
    public class ConfiguracionSistemaModel : PageModel
    {
        private readonly string _connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        [BindProperty]
        public List<ConfiguracionItem> Configuraciones { get; set; }

        public void OnGet()
        {
            Configuraciones = new List<ConfiguracionItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT configuracionSistemaId, clave, valor, descripcion FROM configuracionSistema";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Configuraciones.Add(new ConfiguracionItem
                        {
                            ConfiguracionSistemaId = reader.GetInt32(0),
                            Clave = reader.GetString(1),
                            Valor = reader.GetString(2),
                            Descripcion = reader.GetString(3)
                        });
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                foreach (var config in Configuraciones)
                {
                    string update = @"UPDATE configuracionSistema SET valor = @valor WHERE configuracionSistemaId = @id";

                    using (SqlCommand cmd = new SqlCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@valor", config.Valor);
                        cmd.Parameters.AddWithValue("@id", config.ConfiguracionSistemaId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            TempData["Mensaje"] = "✔ Configuración guardada correctamente.";
            return RedirectToPage("/Administrador/ConfiguracionSistema");
        }

        public class ConfiguracionItem
        {
            public int ConfiguracionSistemaId { get; set; }
            public string Clave { get; set; }
            public string Valor { get; set; }
            public string Descripcion { get; set; }
        }
    }
}
