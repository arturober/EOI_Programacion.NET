namespace Biblioteca.Modelos;

// Define las colecciones disponibles desde el menú Explorar.
public enum TipoListado
{
    Tendencias,
    MejorValorados,
    Novedades,
    Fantasia,
    Misterio,
    CienciaFiccion,
    Romance,
    Programacion
}

public static class TipoListadoExtensiones
{
    public static string Titulo(this TipoListado tipo) => tipo switch
    {
        TipoListado.Tendencias => "Tendencias",
        TipoListado.MejorValorados => "Mejor valorados",
        TipoListado.Novedades => "Publicados recientemente",
        TipoListado.Fantasia => "Fantasía",
        TipoListado.Misterio => "Misterio",
        TipoListado.CienciaFiccion => "Ciencia ficción",
        TipoListado.Romance => "Romance",
        TipoListado.Programacion => "Programación",
        _ => "Libros"
    };

    public static string ParaUrl(this TipoListado tipo) => tipo switch
    {
        TipoListado.MejorValorados => "mejor-valorados",
        TipoListado.Novedades => "novedades",
        TipoListado.Fantasia => "fantasia",
        TipoListado.Misterio => "misterio",
        TipoListado.CienciaFiccion => "ciencia-ficcion",
        TipoListado.Romance => "romance",
        TipoListado.Programacion => "programacion",
        _ => "tendencias"
    };

    public static TipoListado DesdeTexto(string? texto) =>
        texto?.Trim().ToLowerInvariant() switch
        {
            "mejor-valorados" => TipoListado.MejorValorados,
            "novedades" => TipoListado.Novedades,
            "fantasia" => TipoListado.Fantasia,
            "misterio" => TipoListado.Misterio,
            "ciencia-ficcion" => TipoListado.CienciaFiccion,
            "romance" => TipoListado.Romance,
            "programacion" => TipoListado.Programacion,
            _ => TipoListado.Tendencias
        };
}
