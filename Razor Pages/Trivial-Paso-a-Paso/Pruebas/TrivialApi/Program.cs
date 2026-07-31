using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddControllers();

builder.Services.AddDbContext<TrivialContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Trivial")));

WebApplication app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.MapRazorPages();

app.Run();

// WebApplicationFactory necesita acceder a la clase Program desde el proyecto
// de pruebas. En las aplicaciones con instrucciones de nivel superior,
// Program se genera como una clase interna; esta declaración la hace pública.
public partial class Program
{
}
