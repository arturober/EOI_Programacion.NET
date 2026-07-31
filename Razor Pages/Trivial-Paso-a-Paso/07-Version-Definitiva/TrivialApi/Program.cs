using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddControllers();

builder.Services.AddDbContext<TrivialContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Trivial")));


builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirTodo", politica =>
    {
        politica.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("PermitirTodo");

app.MapControllers();

app.MapRazorPages();

app.Run();