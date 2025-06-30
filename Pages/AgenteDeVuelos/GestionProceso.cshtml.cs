using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public List<Vuelo> VuelosList { get; set; }  // Lista de vuelos

        public GestionProcesoModel()
        {
            Usuarios = new Usuario();
            Vuelos = new Vuelo();
            UsuariosList = new List<Usuario>();
            VuelosList = new List<Vuelo>();
        }
        public async Task<IActionResult> OnPostAsync(int usuarioId, string codigoVuelo, string tipoPago)
        {
            if (string.IsNullOrEmpty(codigoVuelo) || usuarioId == 0)
            {
                ModelState.AddModelError("", "Usuario y vuelo son obligatorios.");
                return Page();
            }

            // Obtener vuelo para calcular precio, categoría y descuento
            await ObtenerVueloAsync(codigoVuelo);
            await ObtenerUsuarioAsync(usuarioId);

            TotalPago = Vuelos.PrecioUSD + Vuelos.PrecioPEN + Vuelos.Tarifaespecial;
            Categoria = TotalPago > 500
                ? (TotalPago <= 1200 ? "Vip" : (TotalPago <= 2500 ? "SVip" : "SVIP"))
                : "Bajo";
            Descuento = Categoria == "Vip" ? 10.00m : (Categoria == "SVip" ? 15.00m : 20.00m);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string insertQuery = @"
            INSERT INTO Reservas (usuarioId, vueloId, fechaReserva, estadoReserva, totalPago, fechaModificacion, tipoPago, descuento, categoria)
            VALUES (@usuarioId, @vueloId, @fechaReserva, @estadoReserva, @totalPago, @fechaModificacion, @tipoPago, @descuento, @categoria);
        ";

                SqlCommand cmd = new SqlCommand(insertQuery, connection);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@vueloId", Vuelos.VueloId);
                cmd.Parameters.AddWithValue("@fechaReserva", DateTime.Now);
                cmd.Parameters.AddWithValue("@estadoReserva", "Pendiente");
                cmd.Parameters.AddWithValue("@totalPago", TotalPago);
                cmd.Parameters.AddWithValue("@fechaModificacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@tipoPago", tipoPago);
                cmd.Parameters.AddWithValue("@descuento", Descuento);
                cmd.Parameters.AddWithValue("@categoria", Categoria);

                await cmd.ExecuteNonQueryAsync();
            }

            // Redirigir a la página de pasarela de pago
            return RedirectToPage("/AgenteDeVuelos/GestionPagos", new { usuarioId = usuarioId, codigoVuelo = codigoVuelo });
        }

        public async Task<IActionResult> OnGetAsync(int? usuarioId, string codigoVuelo)
        {
            // Siempre carga los usuarios para el combo
            await ObtenerUsuariosAsync();

            if (!string.IsNullOrEmpty(codigoVuelo))
            {
                CodigoVuelo = codigoVuelo; 
                await ObtenerVueloAsync(codigoVuelo); 
            }


            // Si se pasó el usuario, obtener detalles
            if (usuarioId.HasValue)
            {
                await ObtenerUsuarioAsync(usuarioId.Value);

                // Calcular el total de pago solo si hay vuelo también
                if (Vuelos != null && Vuelos.CodigoVuelo != null)
                {
                    TotalPago = Vuelos.PrecioUSD + Vuelos.PrecioPEN + Vuelos.Tarifaespecial;
                    Categoria = TotalPago > 500 ? (TotalPago <= 1200 ? "Vip" : (TotalPago <= 2500 ? "SVip" : "SVIP")) : "Bajo";
                    Descuento = Categoria == "Vip" ? 10.00m : (Categoria == "SVip" ? 15.00m : 20.00m);
                }
            }

            return Page();
        }



        // Método para obtener los detalles de un usuario específico
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
                    Usuarios = new Usuario
                    {
                        UsuarioId = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Email = reader.GetString(2),
                        Telefono = reader.GetString(3),
                        Direccion = reader.GetString(4)
                    };
                }

                reader.Close();
            }
        }

        // Método para obtener todos los usuarios
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

                // Limpiar la lista antes de llenarla
                UsuariosList.Clear();

                while (await reader.ReadAsync())
                {
                    UsuariosList.Add(new Usuario
                    {
                        UsuarioId = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Email = reader.GetString(2),
                        Telefono = reader.GetString(3),
                        Direccion = reader.GetString(4)
                    });
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

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = @"
                SELECT v.VueloId, v.CodigoVuelo, v.Aerolinea, a.Ciudad, a.Pais, 
                       v.precioUSD, v.precioPEN, v.tarifaEspecial
                FROM Vuelos v
                INNER JOIN Aeropuertos a ON a.AeropuertoId = v.origenId
                WHERE v.CodigoVuelo = @CodigoVuelo
            ";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@CodigoVuelo", codigoVuelo);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    VuelosList.Clear();

                    if (await reader.ReadAsync())
                    {
                        var vuelo = new Vuelo
                        {
                            VueloId = reader.GetInt32(0),
                            CodigoVuelo = reader.GetString(1),
                            Aerolinea = reader.GetString(2),
                            CiudadOrigen = reader.GetString(3),
                            PaisOrigen = reader.GetString(4),
                            PrecioUSD = reader.GetDecimal(5),
                            PrecioPEN = reader.GetDecimal(6),
                            Tarifaespecial = reader.GetDecimal(7)
                        };

                        VuelosList.Add(vuelo);
                        Vuelos = vuelo; // ← Esta línea es clave para que se muestre en la vista
                    }
                    else
                    {
                        throw new Exception("Vuelo no encontrado.");
                    }

                    reader.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error al obtener el vuelo desde la base de datos: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al procesar la solicitud: " + ex.Message);
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