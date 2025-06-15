using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Configuración de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar otros servicios personalizados
builder.Services.AddScoped<FlightService>();

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

// Agregar autenticación y autorización
app.UseAuthentication(); // Se coloca aquí, antes de UseAuthorization
app.UseAuthorization();  // Se coloca aquí, después de UseAuthentication

app.MapRazorPages();

app.Run();
