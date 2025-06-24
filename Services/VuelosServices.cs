using Dapper;
using Microsoft.Data.SqlClient;
using Xpectrum_Structure.ViewModels;

public class VuelosService
{
    private readonly IConfiguration _configuration;
    public VuelosService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<Aeropuerto>> GetAeropuertosAsync()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var query = @"SELECT a.AeropuertoId, a.Nombre, a.Ciudad, a.Pais
                          FROM Aeropuertos a
                          INNER JOIN Vuelos v ON a.AeropuertoId = v.origenId OR a.AeropuertoId = v.destinoId
                          GROUP BY a.AeropuertoId, a.Nombre, a.Ciudad, a.Pais";
            return (await connection.QueryAsync<Aeropuerto>(query)).ToList();
        }
    }
}
