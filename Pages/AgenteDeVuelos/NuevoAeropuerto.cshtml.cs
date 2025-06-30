using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Xpectrum_Structure.Pages.Aeropuertos
{
    public class NuevoAeropuertoModel : PageModel
    {
        private readonly string _connectionString = "workstation id=xpectrum.mssql.somee.com;packet size=4096;user id=KKevinyouman2004_SQLLogin_2;pwd=Kevinyouman2004;data source=xpectrum.mssql.somee.com;persist security info=False;initial catalog=xpectrum;TrustServerCertificate=True";
        private readonly IWebHostEnvironment _env;

        [BindProperty]
        public AeropuertoInput Aeropuerto { get; set; }

        [BindProperty]
        public IFormFile Imagen { get; set; }

        public NuevoAeropuertoModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public class AeropuertoInput
        {
            [Required]
            public string CodigoIata { get; set; }
            [Required]
            public string Nombre { get; set; }
            [Required]
            public string Ciudad { get; set; }
            [Required]
            public string Pais { get; set; }
            public decimal? Latitud { get; set; }
            public decimal? Longitud { get; set; }
            public string ZonaHoraria { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                string rutaImagen = null;

                if (Imagen != null && Imagen.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(Imagen.FileName).ToLowerInvariant();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError(string.Empty, "Solo se permiten imágenes JPG, PNG o GIF.");
                        return Page();
                    }

                    if (Imagen.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "La imagen excede el tamaño máximo permitido (5 MB).");
                        return Page();
                    }

                    var nombreOriginal = Path.GetFileNameWithoutExtension(Imagen.FileName);
                    var extensionArchivo = Path.GetExtension(Imagen.FileName);
                    var nombreLimpio = string.Concat(nombreOriginal.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
                    if (nombreLimpio.Length > 100)
                        nombreLimpio = nombreLimpio.Substring(0, 100);

                    var rutaCarpeta = Path.Combine(_env.WebRootPath, "img", "aeropuertos");
                    if (!Directory.Exists(rutaCarpeta))
                        Directory.CreateDirectory(rutaCarpeta);

                    string nombreFinal = $"{nombreLimpio}{extensionArchivo}";
                    string rutaDestino = Path.Combine(rutaCarpeta, nombreFinal);
                    int contador = 1;

                    while (System.IO.File.Exists(rutaDestino))
                    {
                        nombreFinal = $"{nombreLimpio}_{contador}{extensionArchivo}";
                        rutaDestino = Path.Combine(rutaCarpeta, nombreFinal);
                        contador++;
                    }

                    using (var stream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        await Imagen.CopyToAsync(stream);
                    }

                    rutaImagen = $"/img/aeropuertos/{nombreFinal}";
                }

                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                var query = @"
                    INSERT INTO Aeropuertos (codigoIata, nombre, ciudad, pais, latitud, longitud, zonaHoraria, imagen)
                    VALUES (@codigoIata, @nombre, @ciudad, @pais, @latitud, @longitud, @zonaHoraria, @imagen)";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@codigoIata", Aeropuerto.CodigoIata);
                cmd.Parameters.AddWithValue("@nombre", Aeropuerto.Nombre);
                cmd.Parameters.AddWithValue("@ciudad", Aeropuerto.Ciudad);
                cmd.Parameters.AddWithValue("@pais", Aeropuerto.Pais);
                cmd.Parameters.AddWithValue("@latitud", (object?)Aeropuerto.Latitud ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@longitud", (object?)Aeropuerto.Longitud ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@zonaHoraria", (object?)Aeropuerto.ZonaHoraria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@imagen", (object?)rutaImagen ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();

                TempData["SuccessMessage"] = "Aeropuerto registrado exitosamente.";
                return RedirectToPage("/AgenteDeVuelos/GestionVuelos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar aeropuerto: {ex.Message}");
                return Page();
            }
        }
    }
}

