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
    [HttpGet]
    public async Task<List<PreguntaDto>> ObtenerTodas(
        int? categoriaId, int cantidad = 10)
    {
        cantidad = Math.Clamp(cantidad, 1, 1000);

        IQueryable<Pregunta> consulta = contexto.Preguntas
            .Include(p => p.Categoria);

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);
        }
        
        List<Pregunta> preguntas = await consulta.ToListAsync();

        return preguntas
            .OrderBy(_ => Random.Shared.Next())
            .Take(cantidad)
            .Select(ConvertirDto)
            .ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PreguntaDto>> ObtenerPorId(int id)
    {
        Pregunta? pregunta = await contexto.Preguntas
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pregunta is not null ? Ok(ConvertirDto(pregunta)) : NotFound();
    }

    private static PreguntaDto ConvertirDto(Pregunta pregunta)
    {
        string[] respuestas = [
            pregunta.Respuesta1,
            pregunta.Respuesta2,
            pregunta.Respuesta3,
            pregunta.Respuesta4  
        ];

        CategoriaDto categoria = new CategoriaDto(
            pregunta.CategoriaId, pregunta.Categoria!.Nombre);

        return new PreguntaDto(
            pregunta.Id, pregunta.Enunciado, respuestas,
            pregunta.RespuestaCorrecta, categoria);
    }
}
