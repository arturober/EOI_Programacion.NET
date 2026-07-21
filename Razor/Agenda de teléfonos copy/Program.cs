using AgendaTelefonos.Datos;
using AgendaTelefonos.Models;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

// Comprobamos al arrancar que la tabla y sus columnas existen.
using (SqliteConnection conexion = BaseDatos.Inicializar())
{
    Persona.PrepararTabla(conexion);
}

app.UseRouting();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
