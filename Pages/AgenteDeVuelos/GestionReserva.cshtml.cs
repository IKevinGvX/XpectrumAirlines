using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Xpectrum_Structure.ViewModels;
using static Xpectrum_Structure.Pages.AgenteDeVuelos.GestionReservasModel;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class GestionReservasModel : PageModel
    {
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";
        public List<AeropuertoVuelos> AeropuertosVueloss { get; set; }

        public List<Aeropuertos> Aeropuerto { get; set; }
        public Usuario Usuarios { get; set; }
        public Vuelo Vuelos { get; set; }

        public GestionReservasModel()
        {
            AeropuertosVueloss = new List<AeropuertoVuelos>();  // Inicialización de la lista
            Aeropuerto = new List<Aeropuertos>();
            Usuarios = new Usuario();
            Vuelos = new Vuelo();
        }

        // Usamos solo OnGetAsync para manejar la solicitud asincrónica
        public async Task OnGetAsync(int usuarioId, int vueloId)
        {
            await ObtenerUsuarioAsync(usuarioId);
            await ObtenerVueloAsync(vueloId);
            await ObtenerAeropuertosAsync();
            await ObtenerAeropuertosVuelosAsync();
        }

        private async Task ObtenerAeropuertosVuelosAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                SELECT DISTINCT
    a.AeropuertoId, 
    a.Nombre, 
    a.Ciudad, 
    a.Pais, 
    v.CodigoVuelo, 
    v.Aerolinea
FROM 
    Aeropuertos a
INNER JOIN 
    Vuelos v 
    ON a.AeropuertoId = v.origenId OR a.AeropuertoId = v.destinoId;

            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                // Verificación para asegurar que se está obteniendo información
                if (!reader.HasRows)
                {
                    // Esto te ayudará a saber si la consulta no devuelve resultados
                    Console.WriteLine("No se encontraron vuelos o aeropuertos.");
                }

                while (await reader.ReadAsync())
                {
                    AeropuertosVueloss.Add(new AeropuertoVuelos
                    {
                        AeropuertoId = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Ciudad = reader.GetString(2),
                        Pais = reader.GetString(3),
                        CodigoVuelo = reader.GetString(4),
                        Aerolinea = reader.GetString(5)
                    });
                }

                reader.Close();
            }
        }

        private async Task ObtenerUsuarioAsync(int usuarioId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                SELECT nombre, email, telefono, direccion, preferenciasNotificaciones
                FROM Usuarios
                WHERE usuarioId = @UsuarioId
            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    Usuarios = new Usuario
                    {
                        Nombre = reader.GetString(0),
                        Email = reader.GetString(1),
                        Telefono = reader.GetString(2),
                        Direccion = reader.GetString(3),
                        PreferenciasNotificaciones = reader.GetString(4)
                    };
                }

                reader.Close();
            }
        }

        private async Task ObtenerVueloAsync(int vueloId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                SELECT v.VueloId, v.CodigoVuelo, v.Aerolinea, a.Ciudad, a.Pais
                FROM Vuelos v
                INNER JOIN Aeropuertos a ON a.AeropuertoId = v.origenId
                WHERE v.VueloId = @VueloId
            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@VueloId", vueloId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    Vuelos = new Vuelo
                    {
                        VueloId = reader.GetInt32(0),
                        CodigoVuelo = reader.GetString(1),
                        Aerolinea = reader.GetString(2),
                        CiudadOrigen = reader.GetString(3),
                        PaisOrigen = reader.GetString(4)
                    };
                }

                reader.Close();
            }
        }

        // Método para obtener los aeropuertos
        private async Task ObtenerAeropuertosAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                SELECT AeropuertoId, Nombre, Ciudad, Pais
                FROM Aeropuertos
            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Aeropuerto.Add(new Aeropuertos
                    {
                        AeropuertoId = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Ciudad = reader.GetString(2),
                        Pais = reader.GetString(3)
                    });
                }

                reader.Close();
            }
        }

        public async Task<IActionResult> OnPostCrearReservaAsync(int usuarioId, int vueloId, string tipoPago, double totalPago)
        {
            string categoria = totalPago > 500 ? (totalPago <= 1200 ? "Vip" : (totalPago <= 2500 ? "SVip" : "SVIP")) : "Bajo";
            double descuento = categoria == "Vip" ? 10.00 : (categoria == "SVip" ? 15.00 : 20.00);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                INSERT INTO Reservas (usuarioId, vueloId, fechaReserva, estadoReserva, totalPago, tipoPago, descuento, categoria)
                VALUES (@UsuarioId, @VueloId, GETDATE(), 'Pendiente', @TotalPago, @TipoPago, @Descuento, @Categoria)
            ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@VueloId", vueloId);
                cmd.Parameters.AddWithValue("@TotalPago", totalPago);
                cmd.Parameters.AddWithValue("@TipoPago", tipoPago);
                cmd.Parameters.AddWithValue("@Descuento", descuento);
                cmd.Parameters.AddWithValue("@Categoria", categoria);

                await cmd.ExecuteNonQueryAsync();
            }

            return RedirectToPage("./ConfirmacionReserva");
        }



        public class Aeropuertos
        {
            public int AeropuertoId { get; set; }
            public string Nombre { get; set; }
            public string Ciudad { get; set; }
            public string Pais { get; set; }
        }


        public class Usuario
        {
            public string Nombre { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public string PreferenciasNotificaciones { get; set; }
        }
        public class AeropuertoVuelos
        {
            public int AeropuertoId { get; set; }
            public string Nombre { get; set; }
            public string Ciudad { get; set; }
            public string Pais { get; set; }
            public string CodigoVuelo { get; set; }
            public string Aerolinea { get; set; }
        }

        public class Vuelo
        {
            public int VueloId { get; set; }
            public string CodigoVuelo { get; set; }
            public string Aerolinea { get; set; }
            public string CiudadOrigen { get; set; }
            public string PaisOrigen { get; set; }
            public decimal PrecioUSD { get; set; }
            public decimal PrecioPEN { get; set; }

        }
    }


}