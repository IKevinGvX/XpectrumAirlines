using QRCoder;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Xpectrum_Structure.Pages.Promotions
{
    public class Vuelos : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string apiUrl = "http://apiswagger.somee.com/api/Vuelos/Vuelosxpasajeros"; // URL configurada directamente

        public List<VueloViewModel> Vuelitos { get; set; }

        // Constructor donde inyectamos IHttpClientFactory
        public Vuelos(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            Vuelitos = await ObtenerVuelosDesdeApi();
        }

        private async Task<List<VueloViewModel>> ObtenerVuelosDesdeApi()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Se usa la URL completa directamente
                var response = await client.GetStringAsync(apiUrl).ConfigureAwait(false);

                // Deserializar la respuesta de la API a una lista de objetos VueloViewModel
                var vuelos = JsonConvert.DeserializeObject<List<VueloViewModel>>(response);

                return vuelos ?? new List<VueloViewModel>(); // Retorna una lista vacía si la deserialización falla
            }
            catch (HttpRequestException e)
            {
                // Manejo básico de errores
                Console.WriteLine($"Error al realizar la petición: {e.Message}");
                return new List<VueloViewModel>(); // Retorna una lista vacía en caso de error
            }
            catch (JsonException e)
            {
                // Manejo de errores de deserialización
                Console.WriteLine($"Error al deserializar la respuesta: {e.Message}");
                return new List<VueloViewModel>(); // Retorna una lista vacía en caso de error
            }
        }
    }

}