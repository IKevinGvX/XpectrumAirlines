using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xpectrum_Structure.ViewModels;

public class BoletoService
{
    private readonly HttpClient _httpClient;

    public BoletoService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("XpectrumAPI");
    }

    public async Task<List<BoletoViewModel>> GetBoletosAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<BoletoViewModel>>("boletos/listar");
    }

    public async Task<List<BoletoViewModel>> SearchBoletosAsync(string codigo)
    {
        return await _httpClient.GetFromJsonAsync<List<BoletoViewModel>>($"boletos/searchbycodigo/{codigo}");
    }

    // Métodos POST, PUT, DELETE según necesites
}
