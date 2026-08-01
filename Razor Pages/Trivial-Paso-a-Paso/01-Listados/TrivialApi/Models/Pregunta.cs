using System.ComponentModel.DataAnnotations;

namespace TrivialApi.Models;

// Esta clase representa una pregunta completa almacenada en SQLite.
public class Pregunta
{
    // Id es la clave primaria que permite distinguir cada pregunta.
    public int Id { get; set; }

    // El enunciado es obligatorio y admite como máximo 300 caracteres.
    [Required(ErrorMessage = "La pregunta es obligatoria.")]
    [StringLength(300)]
    [Display(Name = "Pregunta")]
    public string Enunciado { get; set; } = "";

    // Las cuatro respuestas se guardan en columnas independientes.
    // Esta estructura facilita la edición mediante un formulario Razor sencillo.
    [Required(ErrorMessage = "La respuesta 1 es obligatoria.")]
    [StringLength(150)]
    [Display(Name = "Respuesta 1")]
    public string Respuesta1 { get; set; } = "";

    [Required(ErrorMessage = "La respuesta 2 es obligatoria.")]
    [StringLength(150)]
    [Display(Name = "Respuesta 2")]
    public string Respuesta2 { get; set; } = "";

    [Required(ErrorMessage = "La respuesta 3 es obligatoria.")]
    [StringLength(150)]
    [Display(Name = "Respuesta 3")]
    public string Respuesta3 { get; set; } = "";

    [Required(ErrorMessage = "La respuesta 4 es obligatoria.")]
    [StringLength(150)]
    [Display(Name = "Respuesta 4")]
    public string Respuesta4 { get; set; } = "";

    // Guardamos un número del 1 al 4 para indicar qué respuesta es correcta.
    // Range evita que se pueda guardar cualquier otro número.
    [Range(1, 4, ErrorMessage = "Selecciona la respuesta correcta.")]
    [Display(Name = "Respuesta correcta")]
    public int RespuestaCorrecta { get; set; } = 1;

    // CategoriaId es la clave foránea almacenada en la tabla Preguntas.
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    // Categoria es la propiedad de navegación que permite acceder al objeto relacionado.
    // Puede ser null cuando la consulta no ha cargado expresamente la relación.
    public Categoria? Categoria { get; set; }
}

