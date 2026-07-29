using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<TrivialContext>(opciones =>
    opciones.UseSqlite(
        builder.Configuration.GetConnectionString("Trivial")));

WebApplication app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();