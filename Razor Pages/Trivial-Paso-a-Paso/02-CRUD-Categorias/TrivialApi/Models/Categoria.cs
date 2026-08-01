using System.ComponentModel.DataAnnotations;

namespace TrivialApi.Models;

// Esta clase representa una fila de la tabla Categorias.
public class Categoria
{
    // SQLite genera automáticamente el identificador de cada nueva categoría.
    public int Id { get; set; }

    // Required impide nombres vacíos y StringLength limita su longitud.
    // Display proporciona una etiqueta adecuada para los futuros formularios.
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60)]
    [Display(Name = "Categoría")]
    public string Nombre { get; set; } = "";

    // Esta propiedad de navegación contiene todas las preguntas relacionadas.
    // La colección se inicializa vacía para que nunca sea necesario comprobar null.
    public List<Pregunta> Preguntas { get; set; } = [];
}

