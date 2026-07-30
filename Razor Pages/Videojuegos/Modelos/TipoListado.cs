namespace Videojuegos.Modelos;

// Define las colecciones disponibles desde el menú Explorar.
public enum TipoListado
{
    Populares,
    MejorValorados,
    Novedades,
    Proximamente,
    Accion,
    Rol,
    Estrategia,
    Indie,
    Deportes,
    Carreras
}

public static class TipoListadoExtensiones
{
    public static string Titulo(this TipoListado tipo) => tipo switch
    {
        TipoListado.Populares => "Más populares",
        TipoListado.MejorValorados => "Mejor valorados",
        TipoListado.Novedades => "Novedades",
        TipoListado.Proximamente => "Próximos lanzamientos",
        TipoListado.Accion => "Acción",
        TipoListado.Rol => "Rol",
        TipoListado.Estrategia => "Estrategia",
        TipoListado.Indie => "Independientes",
        TipoListado.Deportes => "Deportes",
        TipoListado.Carreras => "Carreras",
        _ => "Videojuegos"
    };

    public static string ParaUrl(this TipoListado tipo) => tipo switch
    {
        TipoListado.MejorValorados => "mejor-valorados",
        TipoListado.Novedades => "novedades",
        TipoListado.Proximamente => "proximamente",
        TipoListado.Accion => "accion",
        TipoListado.Rol => "rol",
        TipoListado.Estrategia => "estrategia",
        TipoListado.Indie => "indie",
        TipoListado.Deportes => "deportes",
        TipoListado.Carreras => "carreras",
        _ => "populares"
    };

    public static TipoListado DesdeTexto(string? texto) =>
        texto?.Trim().ToLowerInvariant() switch
        {
            "mejor-valorados" => TipoListado.MejorValorados,
            "novedades" => TipoListado.Novedades,
            "proximamente" => TipoListado.Proximamente,
            "accion" => TipoListado.Accion,
            "rol" => TipoListado.Rol,
            "estrategia" => TipoListado.Estrategia,
            "indie" => TipoListado.Indie,
            "deportes" => TipoListado.Deportes,
            "carreras" => TipoListado.Carreras,
            _ => TipoListado.Populares
        };
}
