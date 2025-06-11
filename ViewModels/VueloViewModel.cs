public class VueloViewModel
{
    public string CodigoVuelo { get; set; }
    public DateTime? FechaSalida { get; set; }
    public string HoraSalida { get; set; }
    public DateTime? FechaLlegada { get; set; }
    public string HoraLlegada { get; set; }
    public double? DuracionHoras { get; set; } // Usar Nullable double
    public double? DuracionMinutos { get; set; } // Usar Nullable double
    public string EstadoVueloFinal { get; set; }
    public string AeropuertoOrigen { get; set; }
    public string AeropuertoDestino { get; set; }
    public string AeronaveModelo { get; set; }
    public int AeronaveCapacidad { get; set; }
    public string EstadoVuelo { get; set; }
    public string TipoViaje { get; set; }
    public string Clase { get; set; }
    public string Beneficio { get; set; }
    public double? PrecioUSD { get; set; } // Usar Nullable double
    public double? PrecioPEN { get; set; } // Usar Nullable double
    public string CiudadOrigen { get; set; }
    public string CiudadDestino { get; set; }
    public string Imagen { get; set; }

    // Nuevas propiedades que podrían faltar según el contexto proporcionado:
    public string NombreUsuario { get; set; }  // Nombre del usuario que ha reservado el vuelo
    public string DiaSemana { get; set; }      // Día de la semana del vuelo (por ejemplo: "Sunday")
    public string Aerolinea { get; internal set; }
}
