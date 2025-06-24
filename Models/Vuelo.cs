using System;
using System.ComponentModel.DataAnnotations;

namespace Xpectrum_Structure.Models
{
    public class Vuelo
    {
        [Key]
        public string CodigoVuelo { get; set; }
        public DateTime? FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public DateTime? FechaLlegada { get; set; }
        public string HoraLlegada { get; set; }
        public string EstadoVuelo { get; set; }
        public string Aerolinea { get; set; }
        public double PrecioUSD { get; set; }
        public double PrecioPEN { get; set; }
        public string AeropuertoOrigen { get; set; }
        public string CiudadOrigen { get; set; }
        public string PaisOrigen { get; set; }
    }
}
