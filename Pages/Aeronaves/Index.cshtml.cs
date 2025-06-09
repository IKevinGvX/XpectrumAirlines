using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xpectrum_Structure.Services;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Pages.Aeronaves
{
    public class IndexModel : PageModel
    {
        private readonly AeronaveService _aeronaveService;

        public IndexModel(AeronaveService aeronaveService)
        {
            _aeronaveService = aeronaveService;
        }

        public List<AeronaveViewModel> Aeronaves { get; set; } = new List<AeronaveViewModel>();

        public async Task OnGetAsync()
        {
            Aeronaves = await _aeronaveService.GetAeronavesAsync();
        }
    }
}
