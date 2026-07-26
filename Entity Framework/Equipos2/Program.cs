using Equipos.Data;
using Equipos.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=equipos.db"));

// Registrar Servicios de la Aplicación
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IJugadorService, JugadorService>();

// Configurar Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Asegurar creación de base de datos e inicialización (Seeding)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

// Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
