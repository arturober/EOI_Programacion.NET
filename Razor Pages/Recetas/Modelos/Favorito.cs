namespace Recetas.Modelos;

// Relaciona un usuario con una receta marcada como favorita.
public class Favorito
{
    public string UsuarioId { get; set; } = "";
    public Usuario Usuario { get; set; } = null!;

    public int RecetaId { get; set; }
    public Receta Receta { get; set; } = null!;

    public DateTime FechaAgregadaUtc { get; set; }
}
