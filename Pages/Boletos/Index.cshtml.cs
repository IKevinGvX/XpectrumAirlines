

using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xpectrum_Structure.Services;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Pages.Boletos
{
    public class IndexModel : PageModel
    {
        private readonly BoletoService _boletoService;

        public IndexModel(BoletoService boletoService)
        {
            _boletoService = boletoService;
        }

        public List<BoletoViewModel> Boletos { get; set; } = new();

        public async Task OnGetAsync()
        {
            Boletos = await _boletoService.GetBoletosAsync();
        }
    }
}
