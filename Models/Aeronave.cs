using System.ComponentModel.DataAnnotations;

namespace Xpectrum_Structure.Models
{
    public class Aeronave
    {
        [Key]
        public int AeronaveId { get; set; }
        public string Modelo { get; set; }
        public int Capacidad { get; set; }
        public string Matricula { get; set; }
        public int? AñoFabricacion { get; set; }
        public string TipoAeronave { get; set; }
    }
}
