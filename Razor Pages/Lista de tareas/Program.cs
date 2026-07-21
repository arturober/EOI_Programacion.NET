using ListaTareas.Datos;
using ListaTareas.Models;
using Microsoft.Data.Sqlite;

// Program.cs es el punto de entrada de la aplicación web.

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);

// Añadimos el servicio que permite utilizar páginas Razor.
constructor.Services.AddRazorPages();

WebApplication aplicacion = constructor.Build();

// Los modelos crean sus tablas si todavía no existen.
using (SqliteConnection conexion = BaseDatos.Inicializar())
{
    Categoria.PrepararTabla(conexion);
    Tarea.PrepararTabla(conexion);
}

aplicacion.UseStaticFiles();
aplicacion.UseRouting();
aplicacion.MapRazorPages();

aplicacion.Run();
