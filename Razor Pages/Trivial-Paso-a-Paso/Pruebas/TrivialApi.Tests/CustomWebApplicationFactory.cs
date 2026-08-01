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

// Inicia la aplicación dentro del proceso de pruebas y sustituye la base de
// datos real por una base SQLite en memoria.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // El entorno Testing permite distinguir la ejecución de pruebas de la
        // ejecución normal de la aplicación.
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(servicios =>
        {
            // Eliminamos la configuración del contexto registrada por la API.
            servicios.RemoveAll<
                IDbContextOptionsConfiguration<TrivialContext>>();
            servicios.RemoveAll<DbConnection>();

            // SQLite conserva la base en memoria mientras la conexión permanezca
            // abierta. La conexión se libera al finalizar todas las pruebas.
            servicios.AddSingleton<DbConnection>(_ =>
            {
                SqliteConnection conexion = new("Data Source=:memory:");
                conexion.Open();
                return conexion;
            });

            // Todos los contextos creados durante las pruebas utilizan la misma
            // conexión en memoria en lugar del archivo Data/trivial.db.
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

        // Creamos el esquema e insertamos datos conocidos antes de atender la
        // primera petición HTTP.
        using IServiceScope ambito = host.Services.CreateScope();
        TrivialContext contexto =
            ambito.ServiceProvider.GetRequiredService<TrivialContext>();

        contexto.Database.EnsureCreated();
        InsertarDatosDePrueba(contexto);

        return host;
    }

    private static void InsertarDatosDePrueba(TrivialContext contexto)
    {
        // La comprobación evita duplicar los datos si el host se inicializa más
        // de una vez dentro de la misma ejecución.
        if (contexto.Categorias.Any())
        {
            return;
        }

        Categoria arte = new() { Nombre = "Arte" };
        Categoria ciencia = new() { Nombre = "Ciencia" };
        Categoria cultura = new() { Nombre = "Cultura" };

        contexto.Categorias.AddRange(arte, ciencia, cultura);
        contexto.SaveChanges();

        // Se insertan doce preguntas para poder comprobar el límite
        // predeterminado de diez elementos y los filtros por categoría.
        contexto.Preguntas.AddRange(
            CrearPregunta(
                "¿Quién pintó Las Meninas?",
                "Diego Velázquez", "Francisco de Goya",
                "Pablo Picasso", "El Greco",
                1, arte.Id),
            CrearPregunta(
                "¿En qué ciudad se encuentra el Museo del Prado?",
                "Barcelona", "Madrid", "Sevilla", "Valencia",
                2, arte.Id),
            CrearPregunta(
                "¿A qué movimiento artístico se asocia Claude Monet?",
                "Cubismo", "Surrealismo", "Impresionismo", "Barroco",
                3, arte.Id),
            CrearPregunta(
                "¿Quién esculpió el David renacentista de Florencia?",
                "Donatello", "Bernini", "Rodin", "Miguel Ángel",
                4, arte.Id),
            CrearPregunta(
                "¿Cuál es el planeta más cercano al Sol?",
                "Mercurio", "Venus", "La Tierra", "Marte",
                1, ciencia.Id),
            CrearPregunta(
                "¿Qué gas absorben principalmente las plantas?",
                "Oxígeno", "Nitrógeno", "Dióxido de carbono", "Helio",
                3, ciencia.Id),
            CrearPregunta(
                "¿Cuál es la unidad de la intensidad de corriente eléctrica?",
                "Voltio", "Amperio", "Vatio", "Ohmio",
                2, ciencia.Id),
            CrearPregunta(
                "¿Qué órgano bombea la sangre por el cuerpo humano?",
                "Pulmón", "Hígado", "Cerebro", "Corazón",
                4, ciencia.Id),
            CrearPregunta(
                "¿Quién escribió Don Quijote de la Mancha?",
                "Miguel de Cervantes", "Federico García Lorca",
                "Benito Pérez Galdós", "Antonio Machado",
                1, cultura.Id),
            CrearPregunta(
                "¿En qué país se encuentra el museo del Louvre?",
                "Italia", "Francia", "Grecia", "Portugal",
                2, cultura.Id),
            CrearPregunta(
                "¿Cuál es la capital de Portugal?",
                "Oporto", "Braga", "Lisboa", "Coímbra",
                3, cultura.Id),
            CrearPregunta(
                "¿Qué idioma se habla principalmente en Brasil?",
                "Español", "Francés", "Italiano", "Portugués",
                4, cultura.Id));

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
