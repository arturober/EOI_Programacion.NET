namespace OpenFoodFacts.Modelos;

// Relaciona un usuario con un producto guardado en su colección.
public class Favorito
{
    public string UsuarioId { get; set; } = "";
    public Usuario Usuario { get; set; } = null!;

    public string ProductoCodigo { get; set; } = "";
    public ProductoGuardado Producto { get; set; } = null!;

    public DateTime FechaAgregadoUtc { get; set; }
}
