namespace OpenFoodFacts.Modelos;

// Ofrece una selección pequeña y estable de categorías para la navegación.
public static class CatalogoCategorias
{
    public static IReadOnlyList<CategoriaProducto> Todas { get; } =
        new List<CategoriaProducto>
        {
            new(
                "Bebidas",
                "Beverages",
                "bi-cup-straw",
                "Refrescos, zumos, aguas y otras bebidas."),
            new(
                "Aperitivos",
                "Snacks",
                "bi-bag",
                "Patatas, frutos secos y aperitivos salados."),
            new(
                "Chocolates",
                "Chocolates",
                "bi-grid-3x3-gap",
                "Tabletas, bombones y productos de cacao."),
            new(
                "Cereales de desayuno",
                "Breakfast cereals",
                "bi-sunrise",
                "Copos, muesli y cereales para comenzar el día."),
            new(
                "Yogures",
                "Yogurts",
                "bi-cup",
                "Yogures naturales, de sabores y alternativas vegetales."),
            new(
                "Quesos",
                "Cheeses",
                "bi-triangle",
                "Quesos frescos, curados y otras variedades."),
            new(
                "Panes",
                "Breads",
                "bi-basket",
                "Panes, tostadas y productos de panadería."),
            new(
                "Galletas",
                "Biscuits and cakes",
                "bi-cookie",
                "Galletas, bizcochos y productos dulces."),
            new(
                "Helados",
                "Ice creams",
                "bi-snow",
                "Helados, polos y postres congelados."),
            new(
                "Platos preparados",
                "Meals",
                "bi-egg-fried",
                "Comidas preparadas y listas para consumir."),
            new(
                "Conservas",
                "Canned foods",
                "bi-box",
                "Productos vegetales, pescados y carnes en conserva."),
            new(
                "Alimentos vegetales",
                "Plant-based foods",
                "bi-flower1",
                "Productos de origen vegetal.")
        }.AsReadOnly();

    public static CategoriaProducto? Buscar(string filtro) =>
        Todas.FirstOrDefault(elemento =>
            elemento.Filtro.Equals(
                filtro,
                StringComparison.OrdinalIgnoreCase));
}
