using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class DetallesVueloModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetallesVueloModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Propiedad para mostrar el vuelo seleccionado en la vista
        public Vuelos Vuelo { get; set; }

        // Método que obtiene el vuelo específico por su código
        public async Task<IActionResult> OnGetAsync(string codigoVuelo)
        {
            if (string.IsNullOrEmpty(codigoVuelo))
                return NotFound();

            try
            {
                var response = await _httpClient.GetStringAsync("http://www.apiswagger.somee.com/api/vuelos/getvuelos");
                var vuelos = JsonConvert.DeserializeObject<List<Vuelos>>(response);

                Vuelo = vuelos.FirstOrDefault(v => v.CodigoVuelo == codigoVuelo);

                return Vuelo != null ? Page() : NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar vuelo: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
