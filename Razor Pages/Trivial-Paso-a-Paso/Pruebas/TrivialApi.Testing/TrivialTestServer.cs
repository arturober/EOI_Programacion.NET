using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.Models;

namespace TrivialApi.Testing;

// Inicia la versión definitiva con Kestrel en un puerto libre.
// Se utiliza una base SQLite temporal para no modificar Data/trivial.db.
public sealed class TrivialTestServer : IAsyncDisposable
{
    private readonly ConcurrentQueue<string> _ultimasLineas = new();
    private Process? _proceso;
    private string? _directorioTemporal;

    public Uri BaseAddress { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task StartAsync()
    {
        string carpetaPruebas = EncontrarCarpetaPruebas();
        string proyectoApi = Path.GetFullPath(Path.Combine(
            carpetaPruebas,
            "..",
            "07-Version-Definitiva",
            "TrivialApi",
            "TrivialApi.csproj"));

        if (!File.Exists(proyectoApi))
        {
            throw new FileNotFoundException(
                "No se encuentra 07-Version-Definitiva/TrivialApi/TrivialApi.csproj. " +
                "La carpeta Pruebas debe permanecer dentro de Trivial-Paso-a-Paso.",
                proyectoApi);
        }

        _directorioTemporal = Path.Combine(
            Path.GetTempPath(),
            "TrivialApiTests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_directorioTemporal);

        string baseDatos = Path.Combine(_directorioTemporal, "trivial-pruebas.db");
        await CrearBaseDeDatosAsync(baseDatos);

        int puerto = ObtenerPuertoLibre();
        BaseAddress = new Uri($"http://127.0.0.1:{puerto}");

        string configuracion = ObtenerConfiguracion();
        string dotnet = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";

        ProcessStartInfo inicio = new()
        {
            FileName = dotnet,
            WorkingDirectory = Path.GetDirectoryName(proyectoApi)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        inicio.ArgumentList.Add("run");
        inicio.ArgumentList.Add("--project");
        inicio.ArgumentList.Add(proyectoApi);
        inicio.ArgumentList.Add("--configuration");
        inicio.ArgumentList.Add(configuracion);
        inicio.ArgumentList.Add("--no-build");
        inicio.ArgumentList.Add("--no-launch-profile");
        inicio.ArgumentList.Add("--");
        inicio.ArgumentList.Add("--urls");
        inicio.ArgumentList.Add(BaseAddress.ToString());

        inicio.Environment["ASPNETCORE_ENVIRONMENT"] = "Testing";
        inicio.Environment["DOTNET_ENVIRONMENT"] = "Testing";
        inicio.Environment["ConnectionStrings__Trivial"] =
            $"Data Source={baseDatos}";

        _proceso = new Process { StartInfo = inicio, EnableRaisingEvents = true };
        _proceso.OutputDataReceived += (_, evento) => GuardarLinea(evento.Data);
        _proceso.ErrorDataReceived += (_, evento) => GuardarLinea(evento.Data);

        if (!_proceso.Start())
        {
            throw new InvalidOperationException("No se ha podido iniciar TrivialApi.");
        }

        _proceso.BeginOutputReadLine();
        _proceso.BeginErrorReadLine();

        Client = new HttpClient
        {
            BaseAddress = BaseAddress,
            Timeout = TimeSpan.FromSeconds(15)
        };

        await EsperarHastaDisponibleAsync();
    }

    public async ValueTask DisposeAsync()
    {
        Client?.Dispose();

        if (_proceso is not null)
        {
            try
            {
                if (!_proceso.HasExited)
                {
                    _proceso.Kill(entireProcessTree: true);
                    await _proceso.WaitForExitAsync();
                }
            }
            catch (InvalidOperationException)
            {
                // El proceso ya había terminado.
            }
            finally
            {
                _proceso.Dispose();
            }
        }

        if (_directorioTemporal is not null && Directory.Exists(_directorioTemporal))
        {
            try
            {
                Directory.Delete(_directorioTemporal, recursive: true);
            }
            catch (IOException)
            {
                // Windows puede tardar unos milisegundos en liberar SQLite.
            }
        }
    }

    private async Task EsperarHastaDisponibleAsync()
    {
        DateTime limite = DateTime.UtcNow.AddSeconds(40);

        while (DateTime.UtcNow < limite)
        {
            if (_proceso?.HasExited == true)
            {
                throw new InvalidOperationException(
                    "TrivialApi terminó antes de estar disponible.\n" +
                    string.Join(Environment.NewLine, _ultimasLineas));
            }

            try
            {
                using HttpResponseMessage respuesta =
                    await Client.GetAsync("/api/categorias");

                if (respuesta.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Kestrel todavía no ha terminado de arrancar.
            }
            catch (TaskCanceledException)
            {
                // La siguiente vuelta vuelve a intentarlo.
            }

            await Task.Delay(250);
        }

        throw new TimeoutException(
            "TrivialApi no respondió dentro del tiempo esperado.\n" +
            string.Join(Environment.NewLine, _ultimasLineas));
    }

    private static async Task CrearBaseDeDatosAsync(string ruta)
    {
        DbContextOptions<TrivialContext> opciones =
            new DbContextOptionsBuilder<TrivialContext>()
                .UseSqlite($"Data Source={ruta}")
                .Options;

        await using TrivialContext contexto = new(opciones);
        await contexto.Database.EnsureCreatedAsync();

        Categoria arte = new() { Nombre = "Arte" };
        Categoria ciencia = new() { Nombre = "Ciencia" };
        Categoria cultura = new() { Nombre = "Cultura" };

        contexto.Categorias.AddRange(arte, ciencia, cultura);
        await contexto.SaveChangesAsync();

        contexto.Preguntas.AddRange(
            CrearPregunta("¿Quién pintó Las Meninas?", "Diego Velázquez", "Francisco de Goya", "Pablo Picasso", "El Greco", 1, arte.Id),
            CrearPregunta("¿En qué ciudad se encuentra el Museo del Prado?", "Barcelona", "Madrid", "Sevilla", "Valencia", 2, arte.Id),
            CrearPregunta("¿A qué movimiento artístico se asocia Claude Monet?", "Cubismo", "Surrealismo", "Impresionismo", "Barroco", 3, arte.Id),
            CrearPregunta("¿Quién esculpió el David renacentista de Florencia?", "Donatello", "Bernini", "Rodin", "Miguel Ángel", 4, arte.Id),
            CrearPregunta("¿Cuál es el planeta más cercano al Sol?", "Mercurio", "Venus", "La Tierra", "Marte", 1, ciencia.Id),
            CrearPregunta("¿Qué gas absorben principalmente las plantas?", "Oxígeno", "Nitrógeno", "Dióxido de carbono", "Helio", 3, ciencia.Id),
            CrearPregunta("¿Cuál es la unidad de la intensidad de corriente eléctrica?", "Voltio", "Amperio", "Vatio", "Ohmio", 2, ciencia.Id),
            CrearPregunta("¿Qué órgano bombea la sangre por el cuerpo humano?", "Pulmón", "Hígado", "Cerebro", "Corazón", 4, ciencia.Id),
            CrearPregunta("¿Quién escribió Don Quijote de la Mancha?", "Miguel de Cervantes", "Federico García Lorca", "Benito Pérez Galdós", "Antonio Machado", 1, cultura.Id),
            CrearPregunta("¿En qué país se encuentra el museo del Louvre?", "Italia", "Francia", "Grecia", "Portugal", 2, cultura.Id),
            CrearPregunta("¿Cuál es la capital de Portugal?", "Oporto", "Braga", "Lisboa", "Coímbra", 3, cultura.Id),
            CrearPregunta("¿Qué idioma se habla principalmente en Brasil?", "Español", "Francés", "Italiano", "Portugués", 4, cultura.Id));

        await contexto.SaveChangesAsync();
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

    private static string EncontrarCarpetaPruebas()
    {
        DirectoryInfo? carpeta = new(AppContext.BaseDirectory);

        while (carpeta is not null)
        {
            if (File.Exists(Path.Combine(carpeta.FullName, "TrivialApiConPruebas.slnx")))
            {
                return carpeta.FullName;
            }

            carpeta = carpeta.Parent;
        }

        throw new DirectoryNotFoundException(
            "No se ha encontrado la carpeta Pruebas ni TrivialApiConPruebas.slnx.");
    }

    private static string ObtenerConfiguracion()
    {
        string ruta = AppContext.BaseDirectory;
        return ruta.Contains(
            $"{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}",
            StringComparison.OrdinalIgnoreCase)
            ? "Release"
            : "Debug";
    }

    private static int ObtenerPuertoLibre()
    {
        TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        int puerto = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return puerto;
    }

    private void GuardarLinea(string? linea)
    {
        if (string.IsNullOrWhiteSpace(linea))
        {
            return;
        }

        _ultimasLineas.Enqueue(linea);
        while (_ultimasLineas.Count > 40)
        {
            _ultimasLineas.TryDequeue(out _);
        }
    }
}
