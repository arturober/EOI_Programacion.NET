using OpenWeather.Modelos;

namespace OpenWeather.Servicios;

// La interfaz describe lo que necesita la aplicación sin indicar cómo se obtiene.
public interface IOpenWeatherServicio
{
    // Permite mostrar instrucciones claras cuando todavía no existe una clave.
    bool EstaConfigurado { get; }

    // Convierte un texto como "Alicante" en posibles lugares con coordenadas.
    Task<IReadOnlyList<Lugar>> BuscarLugaresAsync(
        string texto,
        CancellationToken cancellationToken = default);

    // Convierte las coordenadas del navegador en el nombre de una localidad.
    Task<Lugar?> BuscarLugarPorCoordenadasAsync(
        double latitud,
        double longitud,
        CancellationToken cancellationToken = default);

    // Descarga y reúne el tiempo actual, la previsión y la calidad del aire.
    Task<InformeMeteorologico> ObtenerInformeAsync(
        Lugar lugar,
        Unidades unidades,
        CancellationToken cancellationToken = default);
}
