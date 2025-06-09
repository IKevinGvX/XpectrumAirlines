using Microsoft.AspNetCore.Mvc.RazorPages;
using Xpectrum_Structure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Xpectrum_Structure.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        public List<VueloViewModel> Vuelos { get; set; } = new List<VueloViewModel>();

        public IndexModel()
        {
        }

        public async Task OnGetAsync()
        {
            // Obtener todos los vuelos sin paginación
            Vuelos = await ObtenerVuelosAsync() ?? new List<VueloViewModel>();
        }

        public async Task<List<VueloViewModel>> ObtenerVuelosAsync()
        {
            List<VueloViewModel> vuelos = new List<VueloViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand("dbo.Splistadodevuelosinvocados", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        VueloViewModel vuelo = new VueloViewModel
                        {
                            CodigoVuelo = reader["codigoVuelo"].ToString(),
                            FechaSalida = reader["fechaSalida"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSalida"]) : (DateTime?)null,
                            HoraSalida = reader["horaSalida"].ToString(),
                            FechaLlegada = reader["fechaLlegada"] != DBNull.Value ? Convert.ToDateTime(reader["fechaLlegada"]) : (DateTime?)null,
                            HoraLlegada = reader["horaLlegada"].ToString(),
                            DuracionHoras = reader["duracionHoras"] != DBNull.Value ? Convert.ToDouble(reader["duracionHoras"]) : 0,
                            DuracionMinutos = reader["duracionMinutos"] != DBNull.Value ? Convert.ToDouble(reader["duracionMinutos"]) : 0,
                            EstadoVueloFinal = reader["estadoVueloFinal"].ToString(),
                            AeropuertoOrigen = reader["aeropuertoOrigen"].ToString(),
                            AeropuertoDestino = reader["aeropuertoDestino"].ToString(),
                            AeronaveModelo = reader["aeronaveModelo"].ToString(),
                            AeronaveCapacidad = reader["aeronaveCapacidad"] != DBNull.Value ? Convert.ToInt32(reader["aeronaveCapacidad"]) : 0,
                            EstadoVuelo = reader["estadoVuelo"].ToString(),
                            TipoViaje = reader["tipoviaje"].ToString(),
                            Clase = reader["clase"].ToString(),
                            Beneficio = reader["beneficio"].ToString(),
                            PrecioUSD = reader["preciousd"] != DBNull.Value ? Convert.ToDouble(reader["preciousd"]) : 0,
                            PrecioPEN = reader["preciopen"] != DBNull.Value ? Convert.ToDouble(reader["preciopen"]) : 0,
                            CiudadOrigen = reader["ciudadOrigen"].ToString(),
                            CiudadDestino = reader["ciudadDestino"].ToString()
                        };

                        vuelos.Add(vuelo);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return vuelos;
        }
    }
}
