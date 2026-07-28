using System.ComponentModel.DataAnnotations;

namespace TrivialApi.Models;

public class Pregunta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El enunciado es obligatorio")]
    [StringLength(300)]
    [Display(Name = "Pregunta")]
    public string Enunciado { get; set; } = "";

    [Required(ErrorMessage = "La respuesta 1 es obligatoria")]
    [StringLength(150)]
    [Display(Name = "Respuesta 1")]
    public string Respuesta1 { get; set; } = ""; 

    [Required(ErrorMessage = "La respuesta 2 es obligatoria")]
    [StringLength(150)]
    [Display(Name = "Respuesta 2")]
    public string Respuesta2 { get; set; } = "";

    [Required(ErrorMessage = "La respuesta 3 es obligatoria")]
    [StringLength(150)]
    [Display(Name = "Respuesta 3")]
    public string Respuesta3 { get; set; } = "";

    [Required(ErrorMessage = "La respuesta 4 es obligatoria")]
    [StringLength(150)]
    [Display(Name = "Respuesta 4")]
    public string Respuesta4 { get; set; } = "";

    [Range(1, 4, ErrorMessage = "Selecciona la respuesta correcta (1-4)")]
    [Display(Name = "Respuesta correcta")]
    public int RespuestaCorrecta { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }   

    public Categoria? Categoria { get; set; } 
}