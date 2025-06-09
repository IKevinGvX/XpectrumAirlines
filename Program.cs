using Microsoft.EntityFrameworkCore;
using Xpectrum_Structure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient<BoletoService>(client =>
{
    client.BaseAddress = new Uri("http://www.apiswagger.somee.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Registrar otros servicios personalizados
builder.Services.AddScoped<FlightService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapRazorPages();

app.Run();
