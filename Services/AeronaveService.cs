using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Services
{
    public class AeronaveService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AeronaveService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<AeronaveViewModel>> GetAeronavesAsync()
        {
            var client = _httpClientFactory.CreateClient("XpectrumAPI");
            var response = await client.GetFromJsonAsync<List<AeronaveViewModel>>("api/aeronaves"); // Cambia según tu endpoint real
            return response ?? new List<AeronaveViewModel>();
        }
    }
}
