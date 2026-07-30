namespace Recetas.Modelos;

// Representa una cantidad y un ingrediente preparados para la interfaz.
public class IngredienteReceta
{
    public string Nombre { get; set; } = "";
    public string Medida { get; set; } = "";

    public string Texto =>
        string.IsNullOrWhiteSpace(Medida)
            ? Nombre
            : $"{Medida} {Nombre}";

    public string UrlImagen
    {
        get
        {
            string nombre = Uri.EscapeDataString(
                Nombre.Replace(" ", "_"));

            // La imagen completa es compatible con la clave educativa de la API.
            return $"https://www.themealdb.com/images/ingredients/{nombre}.png";
        }
    }
}

// Contiene los datos necesarios para una tarjeta o una copia local.
public class RecetaResumen
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? ImagenUrl { get; set; }
    public string Categoria { get; set; } = "";
    public string Area { get; set; } = "";
    public bool EsFavorita { get; set; }
}

// Añade todos los datos que se muestran en la ficha.
public class RecetaDetalle : RecetaResumen
{
    public string Instrucciones { get; set; } = "";
    public string Etiquetas { get; set; } = "";
    public string UrlYoutube { get; set; } = "";
    public string UrlYoutubeEmbed { get; set; } = "";
    public string Fuente { get; set; } = "";
    public IReadOnlyList<IngredienteReceta> Ingredientes { get; set; } = [];
}

// Describe una categoría incluida en el catálogo.
public class CategoriaReceta
{
    public string Nombre { get; set; } = "";
    public string ImagenUrl { get; set; } = "";
    public string Descripcion { get; set; } = "";
}
