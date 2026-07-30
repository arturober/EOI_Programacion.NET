namespace Recetas.Modelos;

// Representa una línea agrupada de la lista de la compra.
public class ElementoCompra
{
    public string Ingrediente { get; set; } = "";
    public IReadOnlyList<string> Medidas { get; set; } = [];
}
