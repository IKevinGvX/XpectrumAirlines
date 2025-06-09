using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Pages.Promotions
{
    public class HistorialModel : PageModel
    {
        private readonly string connectionString = "Server=xpectrum.mssql.somee.com;Database=xpectrum;User Id=KKevinyouman2004_SQLLogin_2;Password=Kevinyouman2004;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=True;Connection Timeout=30;";

        public List<VueloViewModel> Historiales { get; set; } = new List<VueloViewModel>();
        public List<string> AeropuertosOrigen { get; set; } = new List<string>();
        public List<string> AeropuertosDestino { get; set; } = new List<string>();
        public List<string> CiudadesOrigen { get; set; } = new List<string>();
        public List<string> CiudadesDestino { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            Historiales = await ObtenerVuelosHistorialAsync();
            await ObtenerFiltrosAsync();
        }

        // Obtener los vuelos históricos
        public async Task<List<VueloViewModel>> ObtenerVuelosHistorialAsync()
        {
            List<VueloViewModel> vuelos = new List<VueloViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand("dbo.ListadoAll", connection); // Usamos el nuevo procedimiento almacenado
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

        // Obtener los filtros (valores únicos) para los combobox
        public async Task ObtenerFiltrosAsync()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // Aeropuertos de Origen
                    SqlCommand command = new SqlCommand("SELECT  DISTINCT  nombre FROM dbo.Aeropuertos", connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        AeropuertosOrigen.Add(reader["nombre"].ToString());
                    }
                    reader.Close();

                    // Aeropuertos de Destino
                    command = new SqlCommand("SELECT  DISTINCT  nombre FROM dbo.Aeropuertos", connection);
                    reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        AeropuertosDestino.Add(reader["nombre"].ToString());
                    }
                    reader.Close();

                    // Ciudades de Origen
                    command = new SqlCommand("SELECT DISTINCT ciudad FROM dbo.Aeropuertos", connection);
                    reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        CiudadesOrigen.Add(reader["ciudad"].ToString());
                    }
                    reader.Close();

                    // Ciudades de Destino
                    command = new SqlCommand("SELECT DISTINCT ciudad FROM dbo.Aeropuertos", connection);
                    reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        CiudadesDestino.Add(reader["ciudad"].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}
