namespace TrivialApi.DTOs;

public record PreguntaDto(
    int Id, string Enunciado, int CategoriaId,
    string[] Respuestas, int RespuestaCorrecta,
    CategoriaDto Categoria);
