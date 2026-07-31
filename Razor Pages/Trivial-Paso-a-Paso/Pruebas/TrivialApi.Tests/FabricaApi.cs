using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Tests;

// WebApplicationFactory inicia la API dentro del propio proceso de pruebas.
// No hace falta ejecutar previamente "dotnet run".
public class FabricaApi : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(servicios =>
        {
            // Quitamos la configuración de SQLite utilizada por la aplicación.
            servicios.RemoveAll<
                IDbContextOptionsConfiguration<TrivialContext>>();
            servicios.RemoveAll<DbConnection>();

            // La conexión permanece abierta durante todas las pruebas.
            // Una base SQLite en memoria desaparece al cerrar esta conexión.
            servicios.AddSingleton<DbConnection>(_ =>
            {
                SqliteConnection conexion =
                    new("Data Source=:memory:");
                conexion.Open();
                return conexion;
            });

            servicios.AddDbContext<TrivialContext>((proveedor, opciones) =>
            {
                DbConnection conexion =
                    proveedor.GetRequiredService<DbConnection>();
                opciones.UseSqlite(conexion);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);

        // Creamos las tablas e insertamos datos conocidos antes de ejecutar
        // la primera petición.
        using IServiceScope ambito = host.Services.CreateScope();
        TrivialContext contexto =
            ambito.ServiceProvider.GetRequiredService<TrivialContext>();

        contexto.Database.EnsureCreated();
        InsertarDatosDePrueba(contexto);

        return host;
    }

    private static void InsertarDatosDePrueba(TrivialContext contexto)
    {
        if (contexto.Categorias.Any())
        {
            return;
        }

        Categoria ciencia = new() { Nombre = "Ciencia" };
        Categoria cultura = new() { Nombre = "Cultura" };

        contexto.Categorias.AddRange(ciencia, cultura);
        contexto.SaveChanges();

        contexto.Preguntas.AddRange(
            CrearPregunta(
                "¿Cuál es el planeta más cercano al Sol?",
                "Mercurio", "Venus", "La Tierra", "Marte",
                1, ciencia.Id),
            CrearPregunta(
                "¿Qué gas absorben principalmente las plantas?",
                "Oxígeno", "Nitrógeno", "Dióxido de carbono", "Helio",
                3, ciencia.Id),
            CrearPregunta(
                "¿Quién escribió Don Quijote de la Mancha?",
                "Miguel de Cervantes", "Federico García Lorca",
                "Benito Pérez Galdós", "Antonio Machado",
                1, cultura.Id),
            CrearPregunta(
                "¿En qué país se encuentra el museo del Louvre?",
                "Italia", "Francia", "Grecia", "Portugal",
                2, cultura.Id));

        contexto.SaveChanges();
    }

    private static Pregunta CrearPregunta(
        string enunciado,
        string respuesta1,
        string respuesta2,
        string respuesta3,
        string respuesta4,
        int respuestaCorrecta,
        int categoriaId)
    {
        return new Pregunta
        {
            Enunciado = enunciado,
            Respuesta1 = respuesta1,
            Respuesta2 = respuesta2,
            Respuesta3 = respuesta3,
            Respuesta4 = respuesta4,
            RespuestaCorrecta = respuestaCorrecta,
            CategoriaId = categoriaId
        };
    }
}
