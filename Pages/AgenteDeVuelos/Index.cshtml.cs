using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public Vuelos VueloSeleccionado { get; set; }
        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lista completa de vuelos desde la API
        public IList<Vuelos> Vuelos { get; set; } = new List<Vuelos>();

        // Lista paginada
        public IList<Vuelos> VuelosPaginados { get; set; } = new List<Vuelos>();

        // Página actual recibida desde la URL
        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;

        public int TotalPaginas { get; set; }

        private const int TamanoPagina = 10;

        public async Task OnGetAsync()
        {
            var todos = await ObtenerVuelosDeApiAsync();
            TotalPaginas = (int)Math.Ceiling(todos.Count / (double)TamanoPagina);

            VuelosPaginados = todos
                .Skip((Pagina - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();
        }

        private async Task<IList<Vuelos>> ObtenerVuelosDeApiAsync()
        {
            var vuelos = new List<Vuelos>();

            try
            {
                var response = await _httpClient.GetStringAsync("http://www.apiswagger.somee.com/api/vuelos/getvuelos");
                vuelos = JsonConvert.DeserializeObject<List<Vuelos>>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los vuelos: {ex.Message}");
            }

            return vuelos;
        }

     
    }
}
