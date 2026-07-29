namespace Peliculas.Modelos;

// Identifica las colecciones de películas disponibles en la navegación.
public enum TipoListado
{
    Tendencias,
    EnCartelera,
    Populares,
    MejorValoradas,
    Proximamente
}

public static class TipoListadoExtensiones
{
    public static string Titulo(this TipoListado tipo)
    {
        return tipo switch
        {
            TipoListado.Tendencias => "Tendencias de la semana",
            TipoListado.EnCartelera => "En cartelera",
            TipoListado.Populares => "Películas populares",
            TipoListado.MejorValoradas => "Mejor valoradas",
            TipoListado.Proximamente => "Próximos estrenos",
            _ => "Películas"
        };
    }

    public static string ParaUrl(this TipoListado tipo)
    {
        return tipo switch
        {
            TipoListado.Tendencias => "tendencias",
            TipoListado.EnCartelera => "cartelera",
            TipoListado.Populares => "populares",
            TipoListado.MejorValoradas => "mejor-valoradas",
            TipoListado.Proximamente => "proximamente",
            _ => "populares"
        };
    }

    public static TipoListado DesdeTexto(string? texto)
    {
        return texto?.ToLowerInvariant() switch
        {
            "tendencias" => TipoListado.Tendencias,
            "cartelera" => TipoListado.EnCartelera,
            "mejor-valoradas" => TipoListado.MejorValoradas,
            "proximamente" => TipoListado.Proximamente,
            _ => TipoListado.Populares
        };
    }
}
