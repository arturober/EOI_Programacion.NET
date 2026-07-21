using Microsoft.Data.Sqlite;

// Program.cs es el punto de entrada de la aplicación web.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

WebApplication app = builder.Build();

// Los modelos crean sus tablas y los datos de ejemplo si son necesarios.
using (SqliteConnection conexion = BaseDatos.Inicializar())
{
    Tema.PrepararTabla(conexion);
    Pregunta.PrepararTabla(conexion);
    Pregunta.InsertarDatosIniciales(conexion);
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapRazorPages();

app.Run();
