using Microsoft.EntityFrameworkCore;
using Xpectrum_Structure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Configura HttpClient para BoletoService
builder.Services.AddHttpClient<BoletoService>(client =>
{
    client.BaseAddress = new Uri("https://www.apiswagger.somee.com/"); // Cambi� http a https
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", builder =>
    {
        builder
            .AllowAnyOrigin()              // Permite cualquier origen
            .AllowAnyMethod()              // Permite todos los m�todos HTTP (GET, POST, etc)
            .AllowAnyHeader();            // Permite todas las cabeceras
    });
});

// Configuraci�n de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar otros servicios personalizados
builder.Services.AddScoped<FlightService>();

var app = builder.Build();

// Configuraci�n de error para producci�n
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Configura CORS globalmente
app.UseCors("PermitirTodo");

app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapRazorPages();

app.Run();
