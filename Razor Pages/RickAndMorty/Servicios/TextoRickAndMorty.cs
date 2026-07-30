namespace RickAndMorty.Servicios;

// Traduce los valores cerrados de la API y ayuda a leer sus URL.
public static class TextoRickAndMorty
{
    public static string Estado(string? valor) => valor switch
    {
        "Alive" => "Vivo",
        "Dead" => "Muerto",
        "unknown" => "Desconocido",
        _ => valor ?? "Desconocido"
    };

    public static string Genero(string? valor) => valor switch
    {
        "Female" => "Femenino",
        "Male" => "Masculino",
        "Genderless" => "Sin género",
        "unknown" => "Desconocido",
        _ => valor ?? "Desconocido"
    };

    public static string ClaseEstado(string? valor) => valor switch
    {
        "Alive" => "text-bg-success",
        "Dead" => "text-bg-danger",
        _ => "text-bg-secondary"
    };

    public static int? ExtraerId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        string ultimoSegmento = url.TrimEnd('/')
            .Split('/')
            .Last();

        return int.TryParse(ultimoSegmento, out int id)
            ? id
            : null;
    }
}
