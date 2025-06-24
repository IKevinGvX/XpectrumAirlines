using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class GestionProcesoModel : PageModel
    {
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        public Usuario Usuarios { get; set; }
        public Vuelo Vuelos { get; set; }
        public string CodigoVuelo { get; set; }
        public decimal TotalPago { get; set; }
        public string Categoria { get; set; }
        public decimal Descuento { get; set; }
        public List<Usuario> UsuariosList { get; set; }

        public GestionProcesoModel()
        {
            Usuarios = new Usuario();
            Vuelos = new Vuelo();
            UsuariosList = new List<Usuario>(); // Initialize the list to prevent NullReferenceException
        }
        public async Task<IActionResult> OnGetAsync(int? usuarioId, string codigoVuelo)
        {
            // Asegúrate de que ambos parámetros estén presentes
            if (!usuarioId.HasValue || string.IsNullOrEmpty(codigoVuelo))
            {
                ModelState.AddModelError("", "Debe seleccionar un usuario y un vuelo.");
                return Page(); // Redirige a la misma página
            }

            // Obtén los detalles del usuario seleccionado
            await ObtenerUsuarioAsync(usuarioId.Value);

            // Obtén la lista de usuarios para el dropdown
            await ObtenerUsuariosAsync();

            // Obtén los detalles del vuelo usando el 'codigoVuelo'
            await ObtenerVueloAsync(codigoVuelo);

            // Calcular el total de pago
            TotalPago = Vuelos.PrecioUSD + Vuelos.PrecioPEN + Vuelos.Tarifaespecial;
            Categoria = TotalPago > 500 ? (TotalPago <= 1200 ? "Vip" : (TotalPago <= 2500 ? "SVip" : "SVIP")) : "Bajo";
            Descuento = Categoria == "Vip" ? 10.00m : (Categoria == "SVip" ? 15.00m : 20.00m);

            return Page();
        }



        public async Task ObtenerUsuariosAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
            SELECT usuarioId, nombre, email, telefono, direccion
            FROM Usuarios
        ";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                // Ensure the list is initialized before adding elements
                UsuariosList = new List<Usuario>();

                while (await reader.ReadAsync())
                {
                    UsuariosList.Add(new Usuario
                    {
                        UsuarioId = reader.GetInt32(0),
                        Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Telefono = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Direccion = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                    });
                }

                reader.Close();
            }
        }


        public async Task ObtenerUsuarioAsync(int usuarioId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
            SELECT usuarioId, nombre, email, telefono, direccion
            FROM Usuarios
            WHERE usuarioId = @UsuarioId
        ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Set the Usuarios property to hold the specific user's details
                    Usuarios = new Usuario
                    {
                        UsuarioId = reader.GetInt32(0),
                        Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Telefono = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Direccion = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                    };
                }

                reader.Close();
            }
        }

        private async Task ObtenerVueloAsync(string codigoVuelo)
        {
            if (string.IsNullOrEmpty(codigoVuelo))
            {
                throw new ArgumentException("El código de vuelo no puede ser nulo o vacío.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
            SELECT v.VueloId, v.CodigoVuelo, v.Aerolinea, a.Ciudad, a.Pais, v.precioUSD, v.precioPEN, v.tarifaEspecial
            FROM Vuelos v
            INNER JOIN Aeropuertos a ON a.AeropuertoId = v.origenId
            WHERE v.CodigoVuelo = @CodigoVuelo
        ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@CodigoVuelo", codigoVuelo); // Ensure this parameter is passed
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    Vuelos = new Vuelo
                    {
                        VueloId = reader.GetInt32(0),
                        CodigoVuelo = reader.GetString(1),
                        Aerolinea = reader.GetString(2),
                        CiudadOrigen = reader.GetString(3),
                        PaisOrigen = reader.GetString(4),
                        PrecioUSD = reader.GetDecimal(5),
                        PrecioPEN = reader.GetDecimal(6),
                        Tarifaespecial = reader.GetDecimal(7) // Assuming this column exists
                    };
                }

                reader.Close();
            }
        }


        // Example of the Usuario class
        public class Usuario
        {
            public int UsuarioId { get; set; }
            public string Nombre { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public string PreferenciasNotificaciones { get; set; }
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
            public decimal Tarifaespecial { get; set; } // Assuming you added tarifa column in your query
        }
    }


}