using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{

        public class GestionAeronavesModel : PageModel
        {
        public List<Aeronave> ListaAeronaves { get; set; } = new();

        [BindProperty]
            public Aeronave Aeronave { get; set; }

            private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

            public async Task OnGetAsync()
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    var cmd = new SqlCommand("SELECT * FROM Aeronaves", con);
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        ListaAeronaves.Add(new Aeronave
                        {
                            AeronaveId = reader.GetInt32(0),
                            Modelo = reader.GetString(1),
                            Capacidad = reader.GetInt32(2),
                            Matricula = reader.GetString(3),
                            AñoFabricacion = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                            TipoAeronave = reader.GetString(5)
                        });
                    }
                }
            }

            public async Task<IActionResult> OnPostAsync()
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    var cmd = new SqlCommand("INSERT INTO Aeronaves (Modelo, Capacidad, Matricula, AñoFabricacion, TipoAeronave) VALUES (@Modelo, @Capacidad, @Matricula, @Anio, @Tipo)", con);
                    cmd.Parameters.AddWithValue("@Modelo", Aeronave.Modelo);
                    cmd.Parameters.AddWithValue("@Capacidad", Aeronave.Capacidad);
                    cmd.Parameters.AddWithValue("@Matricula", (object)Aeronave.Matricula ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Anio", (object)Aeronave.AñoFabricacion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipo", (object)Aeronave.TipoAeronave ?? DBNull.Value);
                    await cmd.ExecuteNonQueryAsync();
                }

                return RedirectToPage();
            }

            public async Task<IActionResult> OnPostEditarAsync()
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    var cmd = new SqlCommand("UPDATE Aeronaves SET Modelo = @Modelo, Capacidad = @Capacidad, Matricula = @Matricula, AñoFabricacion = @Anio, TipoAeronave = @Tipo WHERE AeronaveId = @Id", con);
                    cmd.Parameters.AddWithValue("@Modelo", Aeronave.Modelo);
                    cmd.Parameters.AddWithValue("@Capacidad", Aeronave.Capacidad);
                    cmd.Parameters.AddWithValue("@Matricula", (object)Aeronave.Matricula ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Anio", (object)Aeronave.AñoFabricacion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipo", (object)Aeronave.TipoAeronave ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", Aeronave.AeronaveId);
                    await cmd.ExecuteNonQueryAsync();
                }

                return RedirectToPage();
            }

            public async Task<IActionResult> OnGetEliminarAsync(int id)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    var cmd = new SqlCommand("DELETE FROM Aeronaves WHERE AeronaveId = @Id", con);
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                return RedirectToPage();
            }

        }
    }
