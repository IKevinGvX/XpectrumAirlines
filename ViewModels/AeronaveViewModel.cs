namespace Xpectrum_Structure.ViewModels
{
    public class AeronaveViewModel
    {
        
            public int AeronaveId { get; set; }
            public string? Modelo { get; set; }
            public int Capacidad { get; set; }
            public string? Matricula { get; set; }
            public List<VueloViewModel>? Vuelos { get; set; }
        
    }
}
