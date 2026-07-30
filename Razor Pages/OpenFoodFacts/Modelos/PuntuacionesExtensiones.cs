namespace OpenFoodFacts.Modelos;

// Convierte las puntuaciones externas en textos y colores de Bootstrap.
public static class PuntuacionesExtensiones
{
    public static string ClaseNutriScore(this string puntuacion) =>
        puntuacion.ToLowerInvariant() switch
        {
            "a" => "text-bg-success",
            "b" => "text-bg-success",
            "c" => "text-bg-warning",
            "d" => "text-bg-warning",
            "e" => "text-bg-danger",
            _ => "text-bg-secondary"
        };

    public static string TextoNutriScore(this string puntuacion) =>
        string.IsNullOrWhiteSpace(puntuacion)
            ? "Nutri-Score sin datos"
            : $"Nutri-Score {puntuacion.ToUpperInvariant()}";

    public static string ClaseNova(this int? grupo) => grupo switch
    {
        1 => "text-bg-success",
        2 => "text-bg-info",
        3 => "text-bg-warning",
        4 => "text-bg-danger",
        _ => "text-bg-secondary"
    };

    public static string TextoNova(this int? grupo) => grupo switch
    {
        1 => "NOVA 1 · Sin procesar o mínimamente procesado",
        2 => "NOVA 2 · Ingrediente culinario procesado",
        3 => "NOVA 3 · Alimento procesado",
        4 => "NOVA 4 · Producto ultraprocesado",
        _ => "NOVA sin datos"
    };

    public static string ClaseGreenScore(this string puntuacion) =>
        puntuacion.ToLowerInvariant() switch
        {
            "a" or "a-plus" => "text-bg-success",
            "b" => "text-bg-success",
            "c" => "text-bg-warning",
            "d" => "text-bg-warning",
            "e" or "f" => "text-bg-danger",
            _ => "text-bg-secondary"
        };

    public static string TextoGreenScore(this string puntuacion) =>
        string.IsNullOrWhiteSpace(puntuacion)
            ? "Green-Score sin datos"
            : $"Green-Score {puntuacion.ToUpperInvariant()}";
}
