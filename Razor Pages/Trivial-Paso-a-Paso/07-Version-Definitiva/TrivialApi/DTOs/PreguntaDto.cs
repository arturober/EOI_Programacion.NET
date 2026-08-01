namespace TrivialApi.DTOs;

// El DTO agrupa las cuatro respuestas en un array, un formato cómodo para
// recorrer desde JavaScript. La entidad de SQLite mantiene cuatro propiedades
// separadas porque resulta más sencillo editarlas mediante un formulario.
public record PreguntaDto(
    int Id,
    string Enunciado,
    string[] Respuestas,
    int RespuestaCorrecta,
    CategoriaDto Categoria);

