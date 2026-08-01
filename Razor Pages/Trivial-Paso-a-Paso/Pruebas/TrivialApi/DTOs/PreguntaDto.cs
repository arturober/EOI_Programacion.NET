namespace TrivialApi.DTOs;

public record PreguntaDto(
    int Id, string Enunciado, string[] Respuestas,
    int RespuestaCorrecta,
    CategoriaDto Categoria);
