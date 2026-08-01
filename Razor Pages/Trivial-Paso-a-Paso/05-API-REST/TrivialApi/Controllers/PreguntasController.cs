using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.DTOs;
using TrivialApi.Models;

namespace TrivialApi.Controllers;

[ApiController]
[Route("api/preguntas")]
public class PreguntasController(TrivialContext contexto) : ControllerBase
{
    // GET /api/preguntas?categoriaId=2&cantidad=10
    // Los parámetros son opcionales: sin ellos se devuelven diez preguntas
    // aleatorias pertenecientes a cualquier categoría.
    [HttpGet]
    public async Task<List<PreguntaDto>> Obtener(
        int? categoriaId,
        int cantidad = 10)
    {
        // Limitamos la cantidad para evitar respuestas vacías o excesivamente grandes.
        cantidad = Math.Clamp(cantidad, 1, 1000);

        // Include carga la categoría porque su nombre forma parte de PreguntaDto.
        IQueryable<Pregunta> consulta = contexto.Preguntas
            .Include(pregunta => pregunta.Categoria);

        // Si el cliente no proporciona categoría, mantenemos todas las preguntas.
        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(
                pregunta => pregunta.CategoriaId == categoriaId);
        }

        // Ejecutamos la consulta antes de ordenar aleatoriamente.
        // La base educativa solo contiene 1.000 filas, por lo que cargar esta
        // colección completa mantiene el ejemplo fácil de comprender.
        List<Pregunta> preguntas = await consulta.ToListAsync();

        // Mezclamos, recortamos y transformamos las entidades a DTO.
        return preguntas
            .OrderBy(_ => Random.Shared.Next())
            .Take(cantidad)
            .Select(ConvertirDto)
            .ToList();
    }

    // GET /api/preguntas/25
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PreguntaDto>> ObtenerPorId(int id)
    {
        Pregunta? pregunta = await contexto.Preguntas
            .Include(pregunta => pregunta.Categoria)
            .FirstOrDefaultAsync(pregunta => pregunta.Id == id);

        return pregunta is null
            ? NotFound()
            : Ok(ConvertirDto(pregunta));
    }

    private static PreguntaDto ConvertirDto(Pregunta pregunta)
    {
        // Reunimos las cuatro columnas de la entidad en el array de la API.
        string[] respuestas =
        [
            pregunta.Respuesta1,
            pregunta.Respuesta2,
            pregunta.Respuesta3,
            pregunta.Respuesta4
        ];

        // Include garantiza que Categoria está cargada. El operador ! comunica
        // esta garantía al análisis de valores nulos del compilador.
        CategoriaDto categoria = new(
            pregunta.CategoriaId,
            pregunta.Categoria!.Nombre);

        return new PreguntaDto(
            pregunta.Id,
            pregunta.Enunciado,
            respuestas,
            pregunta.RespuestaCorrecta,
            categoria);
    }
}

