namespace Videojuegos.Modelos;

// Representa una página de resultados preparada para la interfaz.
public class PaginaVideojuegos
{
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalResultados { get; set; }
    public IReadOnlyList<VideojuegoResumen> Resultados { get; set; } = [];
}

// Contiene los datos necesarios para una tarjeta y para la copia local.
public class VideojuegoResumen
{
    public int Id { get; set; }
    public string Slug { get; set; } = "";
    public string Nombre { get; set; } = "";
    public DateTime? FechaLanzamiento { get; set; }
    public string? ImagenUrl { get; set; }
    public double? Puntuacion { get; set; }
    public int NumeroValoraciones { get; set; }
    public int? Metacritic { get; set; }
    public int TiempoJuego { get; set; }
    public IReadOnlyList<string> Generos { get; set; } = [];
    public IReadOnlyList<string> Plataformas { get; set; } = [];
    public bool EstaEnBiblioteca { get; set; }

    public string FechaTexto =>
        FechaLanzamiento?.ToString("dd/MM/yyyy") ?? "Sin fecha";

    public string GenerosTexto =>
        Generos.Count > 0 ? string.Join(", ", Generos) : "Sin género";

    public string PlataformasTexto =>
        Plataformas.Count > 0
            ? string.Join(", ", Plataformas.Take(4))
            : "Plataformas no indicadas";

    public string UrlRawg =>
        string.IsNullOrWhiteSpace(Slug)
            ? "https://rawg.io/"
            : $"https://rawg.io/games/{Slug}";
}

// Añade a la ficha la información que no necesita una tarjeta.
public class VideojuegoDetalle : VideojuegoResumen
{
    public string Descripcion { get; set; } = "";
    public string SitioWeb { get; set; } = "";
    public string ClasificacionEdad { get; set; } = "";
    public IReadOnlyList<string> Desarrolladores { get; set; } = [];
    public IReadOnlyList<string> Editores { get; set; } = [];
    public IReadOnlyList<string> Tiendas { get; set; } = [];
    public IReadOnlyList<string> Capturas { get; set; } = [];
}
