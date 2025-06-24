namespace Xpectrum_Structure.ViewModels
{
    public class Vuelos
    {
        public int VueloId { get; set; }
        public string CodigoVuelo { get; set; }
        public int OrigenId { get; set; }
        public int DestinoId { get; set; }
        public DateTime FechaSalida { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public DateTime FechaLlegada { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public string EstadoVuelo { get; set; }
        public int AeronaveId { get; set; }
        public string Aerolinea { get; set; }
        public string CodigoAerolinea { get; set; }
        public string TripulacionCapitan { get; set; }
        public string TripulacionCopiloto { get; set; }
        public string TripulacionAuxiliares { get; set; }
        public int? EscalaId { get; set; }
        public string DuracionEscala { get; set; }
        public string ServiciosAdicionales { get; set; }
        public decimal TarifaEspecial { get; set; }
        public string Moneda { get; set; }
        public string CondicionesAdicionales { get; set; }
        public decimal PrecioUSD { get; set; }
        public decimal PrecioPEN { get; set; }
        public string Imagen { get; set; }

        public string AeropuertoOrigen { get; set; }
        public string CiudadOrigen { get; set; }
        public string PaisOrigen { get; set; }

        // ✅ Nuevas propiedades de destino
        public string CiudadDestino { get; set; }
        public string PaisDestino { get; set; }
        public string AeropuertoDestino { get; set; }
    }
}
