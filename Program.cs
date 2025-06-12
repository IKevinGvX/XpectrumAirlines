using Microsoft.EntityFrameworkCore;
using Xpectrum_Structure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Configura HttpClient para BoletoService
builder.Services.AddHttpClient<BoletoService>(client =>
{
    client.BaseAddress = new Uri("https://www.apiswagger.somee.com/"); // Cambié http a https
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", builder =>
    {
        builder
            .AllowAnyOrigin()              // Permite cualquier origen
            .AllowAnyMethod()              // Permite todos los métodos HTTP (GET, POST, etc)
            .AllowAnyHeader();            // Permite todas las cabeceras
    });
});

// Configuración de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar otros servicios personalizados
builder.Services.AddScoped<FlightService>();

var app = builder.Build();

// Configuración de error para producción
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
