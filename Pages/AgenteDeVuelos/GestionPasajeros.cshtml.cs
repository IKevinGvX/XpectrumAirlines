using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Xpectrum_Structure.ViewModels;

namespace Xpectrum_Structure.Pages.AgenteDeVuelos
{
    public class GestionPasajerosModel : PageModel
    {
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";

        [BindProperty]
        public Usuario Pasajero { get; set; }
        public List<Usuario> ListaPasajeros { get; set; } = new List<Usuario>();
        public async Task<IActionResult> OnGetVuelosAsync(int aeropuertoId)
        {
            var vuelosDisponibles = new List<Vuelos>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                var cmd = new SqlCommand("SELECT VueloId, CodigoVuelo, Aerolinea, CiudadDestino, PaisDestino FROM Vuelos WHERE OrigenId = @AeropuertoId", con);
                cmd.Parameters.AddWithValue("@AeropuertoId", aeropuertoId);

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vuelosDisponibles.Add(new Vuelos
                    {
                        VueloId = reader.GetInt32(0),
                        CodigoVuelo = reader.GetString(1),
                        Aerolinea = reader.GetString(2),
                        CiudadDestino = reader.GetString(3),
                        AeropuertoDestino = reader.GetString(4)
                    });
                }
            }

            return new JsonResult(vuelosDisponibles);
        }



        public async Task OnGetAsync()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var cmd = new SqlCommand("SELECT usuarioId, nombre, email, telefono, direccion, fechaNacimiento, activo, estado FROM usuarios WHERE tipoUsuario = 'usuario'", con);
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ListaPasajeros.Add(new Usuario
                {
                    UsuarioId = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Email = reader.GetString(2),
                    Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Direccion = reader.IsDBNull(4) ? null : reader.GetString(4),
                    FechaNacimiento = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    Activo = reader.GetBoolean(6),
                    Estado = reader.IsDBNull(7) ? "" : reader.GetString(7)
                });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var cmd = new SqlCommand("INSERT INTO usuarios (nombre, email, telefono, direccion, fechaNacimiento, activo, estado, tipoUsuario, contra, fechaRegistro) VALUES (@nombre, @email, @telefono, @direccion, @fechaNacimiento, @activo, @estado, 'usuario', '123', GETDATE())", con);

            cmd.Parameters.AddWithValue("@nombre", Pasajero.Nombre);
            cmd.Parameters.AddWithValue("@email", Pasajero.Email);
            cmd.Parameters.AddWithValue("@telefono", (object?)Pasajero.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)Pasajero.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fechaNacimiento", (object?)Pasajero.FechaNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@activo", Pasajero.Activo ? 1 : 0);  // 1 para 'true', 0 para 'false'
            cmd.Parameters.AddWithValue("@estado", Pasajero.Estado ?? "Activo");

            await cmd.ExecuteNonQueryAsync();
            TempData["Mensaje"] = "Pasajero agregado exitosamente";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditarAsync()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var cmd = new SqlCommand("UPDATE usuarios SET nombre = @nombre, email = @email, telefono = @telefono, direccion = @direccion, fechaNacimiento = @fechaNacimiento, activo = @activo, estado = @estado WHERE usuarioId = @id", con);

            cmd.Parameters.AddWithValue("@nombre", Pasajero.Nombre);
            cmd.Parameters.AddWithValue("@email", Pasajero.Email);
            cmd.Parameters.AddWithValue("@telefono", (object?)Pasajero.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)Pasajero.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fechaNacimiento", (object?)Pasajero.FechaNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@activo", Pasajero.Activo ? 1 : 0);  // 1 para 'true', 0 para 'false'
            cmd.Parameters.AddWithValue("@estado", Pasajero.Estado ?? "Activo");
            cmd.Parameters.AddWithValue("@id", Pasajero.UsuarioId);

            await cmd.ExecuteNonQueryAsync();
            TempData["Mensaje"] = "Pasajero editado correctamente";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetEliminarAsync(int id)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            var cmd = new SqlCommand("DELETE FROM usuarios WHERE usuarioId = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
            TempData["Mensaje"] = "Pasajero eliminado";
            return RedirectToPage();
        }

        public class Usuario
        {
            public int UsuarioId { get; set; }  // Corresponde al ID único del pasajero
            public string Nombre { get; set; }  // Nombre completo del pasajero
            public string Email { get; set; }  // Correo electrónico del pasajero
            public string Telefono { get; set; }  // Teléfono del pasajero
            public string Direccion { get; set; }  // Dirección del pasajero
            public DateTime? FechaNacimiento { get; set; }  // Fecha de nacimiento del pasajero
            public bool Activo { get; set; }  // Estado activo/inactivo del pasajero (true o false)
            public string Estado { get; set; }  // Estado del pasajero (activo, inactivo, etc.)

            // Puedes agregar otras propiedades si se requieren más campos en el futuro.
        }
    }
}