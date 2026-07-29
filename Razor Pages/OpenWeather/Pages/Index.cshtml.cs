using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenWeather.Modelos;
using OpenWeather.Servicios;

namespace OpenWeather.Pages;

public class IndexModel : PageModel
{
    private readonly IOpenWeatherServicio _openWeather;

    public IndexModel(IOpenWeatherServicio openWeather)
    {
        _openWeather = openWeather;
    }

    // El texto se conserva en el formulario después de realizar una búsqueda.
    public string Ciudad { get; private set; } = "";

    // Estos resultados permiten elegir entre localidades con el mismo nombre.
    public IReadOnlyList<Lugar> LugaresEncontrados { get; private set; } = [];

    // Cuando la consulta termina correctamente contiene todo lo que muestra la vista.
    public InformeMeteorologico? Informe { get; private set; }

    // La página de inicio explica cómo añadir la clave si todavía falta.
    public bool EstaConfigurado => _openWeather.EstaConfigurado;

    public async Task OnGetAsync(
        string? ciudad,
        double? lat,
        double? lon,
        string? unidades,
        CancellationToken cancellationToken)
    {
        Ciudad = ciudad?.Trim() ?? "";

        if (!EstaConfigurado)
        {
            // No se intenta llamar a Internet hasta que exista una clave.
            return;
        }

        try
        {
            Lugar? lugar;

            if (lat.HasValue && lon.HasValue)
            {
                // Esta rama se utiliza al elegir un resultado o usar la ubicación.
                lugar = await _openWeather.BuscarLugarPorCoordenadasAsync(
                    lat.Value, lon.Value, cancellationToken);

                lugar ??= new Lugar
                {
                    Nombre = "Ubicación seleccionada",
                    Latitud = lat.Value,
                    Longitud = lon.Value
                };
            }
            else
            {
                // Alicante sirve como ejemplo inicial y puede cambiarse libremente.
                Ciudad = string.IsNullOrWhiteSpace(Ciudad) ? "Alicante" : Ciudad;
                LugaresEncontrados = await _openWeather.BuscarLugaresAsync(
                    Ciudad, cancellationToken);
                lugar = LugaresEncontrados.FirstOrDefault();
            }

            if (lugar is null)
            {
                ViewData["Error"] =
                    "No se ha encontrado ninguna localidad con ese nombre.";
                return;
            }

            Unidades sistemaUnidades = UnidadesExtensiones.DesdeTexto(unidades);

            Informe = await _openWeather.ObtenerInformeAsync(
                lugar, sistemaUnidades, cancellationToken);
        }
        catch (OpenWeatherExcepcion excepcion)
        {
            // El mensaje ya está redactado para que pueda mostrarse al usuario.
            ViewData["Error"] = excepcion.Message;
        }
    }
}
