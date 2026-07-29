using System.ComponentModel.DataAnnotations;

namespace TrivialApi.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage ="El nombre es obligatorio")]
    [StringLength(60)]
    [Display(Name = "Categoría")]
    public string Nombre { get; set; } = "";

    public List<Pregunta> Preguntas { get; set; } = [];
}