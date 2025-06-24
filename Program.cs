using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Configuraci�n de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar otros servicios personalizados
builder.Services.AddScoped<FlightService>();

// Configuraci�n de la autenticaci�n basada en cookies (si la est�s utilizando)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // P�gina de login
        options.LogoutPath = "/Account/Logout"; // P�gina de logout
        options.Cookie.HttpOnly = true;  // Asegura que la cookie no sea accesible desde JavaScript
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Asegura que la cookie solo se env�e por HTTPS
        options.SlidingExpiration = true; // Permite que la sesi�n se renueve
    });

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

// Configuraci�n de localizaci�n (idiomas soportados)
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es-ES"), // Establece el idioma predeterminado a espa�ol
    SupportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("es-ES") },
    SupportedUICultures = new[] { new CultureInfo("en-US"), new CultureInfo("es-ES") }
});

// Configuraci�n de error para producci�n
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // P�gina de errores gen�rica
    app.UseHsts(); // HSTS en producci�n
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Configura CORS globalmente
app.UseCors("PermitirTodo");

app.UseRouting();

// Agregar autenticaci�n y autorizaci�n
app.UseAuthentication(); // Se coloca aqu�, antes de UseAuthorization
app.UseAuthorization();  // Se coloca aqu�, despu�s de UseAuthentication

app.MapRazorPages();

app.Run();
