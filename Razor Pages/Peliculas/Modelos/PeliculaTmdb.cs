namespace Peliculas.Modelos;

// Representa una página de resultados de TMDB.
public class PaginaPeliculas
{
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalResultados { get; set; }
    public IReadOnlyList<PeliculaResumen> Resultados { get; set; } = [];
}

// Contiene los datos necesarios para una tarjeta o para guardar un favorito.
public class PeliculaResumen
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string TituloOriginal { get; set; } = "";
    public string Sinopsis { get; set; } = "";
    public string? RutaPoster { get; set; }
    public string? RutaFondo { get; set; }
    public DateOnly? FechaEstreno { get; set; }
    public double Puntuacion { get; set; }
    public int NumeroVotos { get; set; }
    public bool EsFavorita { get; set; }

    public string Anio =>
        FechaEstreno?.Year.ToString() ?? "Sin fecha";

    public string? UrlPoster =>
        ImagenTmdb.Crear(RutaPoster, "w500");

    public string? UrlPosterPequeno =>
        ImagenTmdb.Crear(RutaPoster, "w342");

    public string? UrlFondo =>
        ImagenTmdb.Crear(RutaFondo, "w1280");
}

// Amplía el resumen con todos los datos de la página de detalles.
public class PeliculaDetalle : PeliculaResumen
{
    public string Eslogan { get; set; } = "";
    public int? DuracionMinutos { get; set; }
    public string Estado { get; set; } = "";
    public string? PaginaOficial { get; set; }
    public string? ImdbId { get; set; }
    public IReadOnlyList<string> Generos { get; set; } = [];
    public IReadOnlyList<string> Paises { get; set; } = [];
    public string? Director { get; set; }
    public IReadOnlyList<PersonaReparto> Reparto { get; set; } = [];
    public VideoTmdb? Trailer { get; set; }
    public IReadOnlyList<PeliculaResumen> Recomendaciones { get; set; } = [];
    public DisponibilidadTmdb? Disponibilidad { get; set; }

    public string DuracionTexto
    {
        get
        {
            if (DuracionMinutos is null or <= 0)
            {
                return "Duración desconocida";
            }

            int horas = DuracionMinutos.Value / 60;
            int minutos = DuracionMinutos.Value % 60;
            return horas > 0 ? $"{horas} h {minutos} min" : $"{minutos} min";
        }
    }

    public string UrlTmdb =>
        $"https://www.themoviedb.org/movie/{Id}";
}

public class PersonaReparto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Personaje { get; set; } = "";
    public string? RutaFoto { get; set; }

    public string? UrlFoto =>
        ImagenTmdb.Crear(RutaFoto, "w185");
}

public class VideoTmdb
{
    public string Nombre { get; set; } = "";
    public string ClaveYoutube { get; set; } = "";

    public string UrlYoutube =>
        $"https://www.youtube.com/watch?v={ClaveYoutube}";
}

public class DisponibilidadTmdb
{
    public string? Enlace { get; set; }
    public IReadOnlyList<ProveedorTmdb> Suscripcion { get; set; } = [];
    public IReadOnlyList<ProveedorTmdb> Alquiler { get; set; } = [];
    public IReadOnlyList<ProveedorTmdb> Compra { get; set; } = [];

    public bool TieneDatos =>
        Suscripcion.Count > 0 || Alquiler.Count > 0 || Compra.Count > 0;
}

public class ProveedorTmdb
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? RutaLogo { get; set; }

    public string? UrlLogo =>
        ImagenTmdb.Crear(RutaLogo, "w92");
}

public static class ImagenTmdb
{
    // Todas las imágenes se construyen aquí para evitar direcciones repetidas.
    public static string? Crear(string? ruta, string tamano)
    {
        if (string.IsNullOrWhiteSpace(ruta))
        {
            return null;
        }

        return $"https://image.tmdb.org/t/p/{tamano}{ruta}";
    }
}
