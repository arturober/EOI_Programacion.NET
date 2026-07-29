namespace Peliculas.Modelos;

// Es la relación entre un usuario local y una película de TMDB.
public class Favorito
{
    public string UsuarioId { get; set; } = "";
    public Usuario Usuario { get; set; } = null!;

    public int PeliculaId { get; set; }
    public Pelicula Pelicula { get; set; } = null!;

    public DateTime FechaAgregadaUtc { get; set; }
}
