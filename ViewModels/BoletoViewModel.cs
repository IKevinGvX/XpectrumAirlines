namespace Xpectrum_Structure.ViewModels
{
    public class BoletoViewModel
    {
        public int BoletoId { get; set; }          // boletoId (PK)
        public int ReservaId { get; set; }         // reservaId (FK)
        public string CodigoBoleto { get; set; }   // codigoBoleto (nvarchar(100))
        public DateTime FechaEmision { get; set; } // fechaEmision (datetime)
        public string EstadoBoleto { get; set; }   // estadoBoleto (nvarchar(50))

        // Código BO (generado dinámicamente, no viene de la BD)
        public string CodigoBO => $"BO-{BoletoId:0000}";

        // URL o base64 del QR (opcional)
        public string QrCodeUrl { get; set; }
    }
}
