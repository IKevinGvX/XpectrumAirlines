using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Xpectrum_Structure.Pages.Aeropuertos;
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
builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Configuración de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, sqlOptions => 
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

// Registrar servicios personalizados
builder.Services.AddScoped<FlightService>();
builder.Services.AddScoped<VueloService>();

// Configuración de la autenticación basada en cookies (si la estás utilizando)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Página de login
        options.LogoutPath = "/Account/Logout"; // Página de logout
        options.Cookie.HttpOnly = true;  // Asegura que la cookie no sea accesible desde JavaScript
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Asegura que la cookie solo se envíe por HTTPS
        options.SlidingExpiration = true; // Permite que la sesión se renueve
    });

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024; // 20MB o más si lo necesitas
});


var app = builder.Build();

// Configuración de localización (idiomas soportados)
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es-ES"), // Establece el idioma predeterminado a español
    SupportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("es-ES") },
    SupportedUICultures = new[] { new CultureInfo("en-US"), new CultureInfo("es-ES") }
});

// Configuración de error para producción
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // Página de errores genérica
    app.UseHsts(); // HSTS en producción
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Configura CORS globalmente
app.UseCors("PermitirTodo");

app.UseRouting();

// Agregar autenticación y autorización
app.UseAuthentication(); // Se coloca aquí, antes de UseAuthorization
app.UseAuthorization();  // Se coloca aquí, después de UseAuthentication

app.MapRazorPages();

app.Run();
