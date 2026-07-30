namespace OpenFoodFacts.Modelos;

// Contiene los datos breves que necesita una tarjeta de producto.
public class ProductoResumen
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Marca { get; set; } = "";
    public string Cantidad { get; set; } = "";
    public string? ImagenUrl { get; set; }
    public string NutriScore { get; set; } = "";
    public int? GrupoNova { get; set; }
    public string GreenScore { get; set; } = "";
    public bool EsFavorito { get; set; }
}

// Representa una fila de la tabla nutricional.
public class NutrienteProducto
{
    public string Nombre { get; set; } = "";
    public double? Cantidad { get; set; }
    public string Unidad { get; set; } = "g";

    public string Texto =>
        Cantidad.HasValue
            ? $"{Cantidad.Value:0.##} {Unidad}"
            : "Sin datos";
}

// Añade toda la información mostrada en la ficha del producto.
public class ProductoDetalle : ProductoResumen
{
    public string NombreGenerico { get; set; } = "";
    public string Ingredientes { get; set; } = "";
    public string Alergenos { get; set; } = "";
    public string Trazas { get; set; } = "";
    public string Categorias { get; set; } = "";
    public string Paises { get; set; } = "";
    public string Etiquetas { get; set; } = "";
    public string Envase { get; set; } = "";
    public string TamanoRacion { get; set; } = "";
    public int? NumeroAditivos { get; set; }
    public IReadOnlyList<string> Aditivos { get; set; } = [];
    public IReadOnlyList<NutrienteProducto> Nutrientes { get; set; } = [];

    // Estas propiedades facilitan guardar y comparar los nutrientes principales.
    public double? EnergiaKcal100g =>
        ObtenerNutriente("Energía")?.Cantidad;
    public double? Grasas100g =>
        ObtenerNutriente("Grasas")?.Cantidad;
    public double? GrasasSaturadas100g =>
        ObtenerNutriente("Grasas saturadas")?.Cantidad;
    public double? Hidratos100g =>
        ObtenerNutriente("Hidratos de carbono")?.Cantidad;
    public double? Azucares100g =>
        ObtenerNutriente("Azúcares")?.Cantidad;
    public double? Fibra100g =>
        ObtenerNutriente("Fibra")?.Cantidad;
    public double? Proteinas100g =>
        ObtenerNutriente("Proteínas")?.Cantidad;
    public double? Sal100g =>
        ObtenerNutriente("Sal")?.Cantidad;

    private NutrienteProducto? ObtenerNutriente(string nombre) =>
        Nutrientes.FirstOrDefault(elemento => elemento.Nombre == nombre);
}

// Reúne resultados y datos de paginación de una búsqueda.
public class ResultadoProductos
{
    public IReadOnlyList<ProductoResumen> Productos { get; set; } = [];
    public long Total { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 12;
    public int TotalPaginas =>
        Total <= 0
            ? 0
            : (int)Math.Ceiling(Total / (double)TamanoPagina);
}

// Describe una categoría popular y el filtro que entiende la API.
public record CategoriaProducto(
    string Nombre,
    string Filtro,
    string Icono,
    string Descripcion);

// Contiene los datos necesarios para reutilizar el paginador.
public class Paginacion
{
    public int Pagina { get; set; }
    public int TotalPaginas { get; set; }
    public string PaginaRazor { get; set; } = "";
    public string? Texto { get; set; }
    public string? Tipo { get; set; }
    public string? Valor { get; set; }
}
