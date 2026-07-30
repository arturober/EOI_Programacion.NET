namespace Biblioteca.Modelos;

// Relaciona un usuario de Identity con una obra de Open Library.
public class Favorito
{
    public string UsuarioId { get; set; } = "";
    public Usuario Usuario { get; set; } = null!;

    public string LibroId { get; set; } = "";
    public Libro Libro { get; set; } = null!;

    public DateTime FechaAgregadoUtc { get; set; }
}
