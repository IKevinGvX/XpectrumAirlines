using System;
using System.ComponentModel.DataAnnotations;

public class VueloViewModel
{
    [Key]
    public string CodigoVuelo { get; set; }  // Esto se convierte en la clave primaria
    public DateTime? FechaSalida { get; set; }
    public string HoraSalida { get; set; }
    public DateTime? FechaLlegada { get; set; }
    public string HoraLlegada { get; set; }
    public double? DuracionHoras { get; set; }
    public double? DuracionMinutos { get; set; }
    public string EstadoVueloFinal { get; set; }
    public string AeropuertoOrigen { get; set; }
    public string AeropuertoDestino { get; set; }
    public string AeronaveModelo { get; set; }
    public int AeronaveCapacidad { get; set; }
    public string EstadoVuelo { get; set; }
    public string TipoViaje { get; set; }
    public string Clase { get; set; }
    public string Beneficio { get; set; }
    public double? PrecioUSD { get; set; }
    public double? PrecioPEN { get; set; }
    public string CiudadOrigen { get; set; }
    public string CiudadDestino { get; set; }
    public string Imagen { get; set; }

    // Nuevas propiedades que podrían faltar según el contexto proporcionado:
    public string NombreUsuario { get; set; }
    public string DiaSemana { get; set; }
    public string Aerolinea { get; set; }
}
