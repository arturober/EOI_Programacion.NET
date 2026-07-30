namespace Biblioteca.Modelos;

// Representa una página de resultados preparada para la interfaz.
public class PaginaLibros
{
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalResultados { get; set; }
    public IReadOnlyList<LibroResumen> Resultados { get; set; } = [];
}

// Contiene los datos necesarios para una tarjeta o un favorito.
public class LibroResumen
{
    public string Id { get; set; } = "";
    public string Titulo { get; set; } = "";
    public IReadOnlyList<string> Autores { get; set; } = [];
    public int? PrimeraPublicacion { get; set; }
    public long? PortadaId { get; set; }
    public int NumeroEdiciones { get; set; }
    public double? Puntuacion { get; set; }
    public int NumeroValoraciones { get; set; }
    public IReadOnlyList<string> Isbn { get; set; } = [];
    public IReadOnlyList<string> Idiomas { get; set; } = [];
    public int? NumeroPaginas { get; set; }
    public string AccesoElectronico { get; set; } = "";
    public bool EsFavorito { get; set; }

    public string AutoresTexto =>
        Autores.Count > 0 ? string.Join(", ", Autores) : "Autor desconocido";

    public string AnioTexto =>
        PrimeraPublicacion?.ToString() ?? "Sin fecha";

    public string? UrlPortada =>
        PortadaId is null
            ? null
            : $"https://covers.openlibrary.org/b/id/{PortadaId}-M.jpg?default=false";

    public string? UrlPortadaGrande =>
        PortadaId is null
            ? null
            : $"https://covers.openlibrary.org/b/id/{PortadaId}-L.jpg?default=false";

    public string UrlOpenLibrary =>
        $"https://openlibrary.org/works/{Id}";
}

// Añade a la ficha la información que no necesita una tarjeta.
public class LibroDetalle : LibroResumen
{
    public string Descripcion { get; set; } = "";
    public string? PrimeraFechaPublicacion { get; set; }
    public IReadOnlyList<string> Materias { get; set; } = [];
    public IReadOnlyList<LibroResumen> Recomendaciones { get; set; } = [];

    public string AccesoTexto => AccesoElectronico switch
    {
        "public" => "Lectura pública disponible",
        "borrowable" => "Disponible para préstamo digital",
        "printdisabled" => "Accesible para usuarios autorizados",
        "no_ebook" => "Sin libro electrónico",
        _ => "Disponibilidad no especificada"
    };
}
