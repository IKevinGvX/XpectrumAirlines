namespace XpectrumStructure.Models
{
    public class Vuelo
    {
        public string CodigoVuelo { get; set; }
        public string FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public string FechaLlegada { get; set; }
        public string HoraLlegada { get; set; }
        public string EstadoVuelo { get; set; }
        public string AeropuertoOrigen { get; set; }
        public string AeropuertoDestino { get; set; }
        public string CiudadOrigen { get; set; }
        public string CiudadDestino { get; set; }
        public string PaisOrigen { get; set; }
        public string PaisDestino { get; set; }
        public string Aerolinea { get; set; }
        public string CondicionesAdicionales { get; set; }
        public decimal PrecioUSD { get; set; }
        public string Moneda { get; set; }
        public decimal TarifaEspecial { get; set; }
        public decimal PrecioPen { get; set; }
        public string Imagen { get; set; }
    }
}
