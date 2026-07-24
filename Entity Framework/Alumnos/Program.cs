using Alumnos.Data;
using Alumnos.Models;
using Alumnos.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=app.db";

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Inyección de dependencias de Servicios
builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<IAsignaturaService, AsignaturaService>();

// Configuración de Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Inicialización y Seeding automático de la Base de Datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    // Cargar datos de prueba si la base de datos está vacía
    if (!await db.Alumnos.AnyAsync())
    {
        var mat = new Asignatura { Nombre = "Matemáticas I", Codigo = "MAT101", Creditos = 6 };
        var prog = new Asignatura { Nombre = "Programación C#", Codigo = "PRG102", Creditos = 9 };
        var bd = new Asignatura { Nombre = "Bases de Datos", Codigo = "ABD103", Creditos = 6 };

        var alumno1 = new Alumno
        {
            Nombre = "Carlos Mendoza",
            Email = "carlos.mendoza@email.com",
            Dni = "12345678A",
            Asignaturas = [mat, prog]
        };

        var alumno2 = new Alumno
        {
            Nombre = "Laura García",
            Email = "laura.garcia@email.com",
            Dni = "87654321B",
            Asignaturas = [prog, bd]
        };

        db.Alumnos.AddRange(alumno1, alumno2);
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.Run();
