using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Xpectrum_Structure.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        // Cambiamos el tipo de Vuelos para que sea directamente List<VueloAPI>
        public List<VueloAPI> Vuelos { get; set; } = new List<VueloAPI>();

        // Constructor
        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Método para obtener vuelos desde la API
        public async Task OnGetAsync()
        {
            // Obtener vuelos de la API
            Vuelos = await ObtenerVuelosDesdeAPI() ?? new List<VueloAPI>();
        }

        // Método para obtener los vuelos de la API
        public async Task<List<VueloAPI>> ObtenerVuelosDesdeAPI()
        {
            List<VueloAPI> vuelos = new List<VueloAPI>();

            try
            {
                // URL de la API
                string apiUrl = "http://www.apiswagger.somee.com/api/vuelos/getvuelos";

                // Realizando la solicitud GET
                var response = await _httpClient.GetStringAsync(apiUrl);

                // Deserializamos la respuesta JSON
                vuelos = JsonConvert.DeserializeObject<List<VueloAPI>>(response);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error al consumir la API: " + ex.Message);
            }

            return vuelos;
        }
    }

    // Modelo para los datos que se reciben de la API
    public class VueloAPI
    {
        public string CodigoVuelo { get; set; }
        public DateTime? FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public DateTime? FechaLlegada { get; set; }
        public string HoraLlegada { get; set; }
        public string EstadoVuelo { get; set; }
        public string Aerolinea { get; set; }
        public double PrecioUSD { get; set; }
        public double PrecioPEN { get; set; }

        // Nuevas propiedades para los datos del aeropuerto de origen
        public string AeropuertoOrigen { get; set; }
        public string CiudadOrigen { get; set; }
        public string PaisOrigen { get; set; }
    }
}
