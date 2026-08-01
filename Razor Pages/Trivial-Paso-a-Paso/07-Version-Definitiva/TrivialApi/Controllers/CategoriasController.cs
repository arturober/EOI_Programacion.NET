using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.DTOs;

namespace TrivialApi.Controllers;

// ApiController activa comportamientos específicos de las API, como respuestas
// automáticas para determinados errores de validación.
[ApiController]

// Todas las acciones de este controlador comienzan por /api/categorias.
[Route("api/categorias")]
public class CategoriasController(TrivialContext contexto) : ControllerBase
{
    // GET /api/categorias
    [HttpGet]
    public async Task<List<CategoriaDto>> ObtenerTodas()
    {
        // Select convierte cada entidad en su DTO dentro de la consulta.
        // SQLite solo devuelve las columnas Id y Nombre que realmente necesitamos.
        return await contexto.Categorias
            .OrderBy(categoria => categoria.Nombre)
            .Select(categoria =>
                new CategoriaDto(categoria.Id, categoria.Nombre))
            .ToListAsync();
    }

    // GET /api/categorias/3
    // La restricción int evita que textos arbitrarios lleguen a esta acción.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> ObtenerPorId(int id)
    {
        // Where filtra por clave y Select mantiene separado el DTO de la entidad.
        CategoriaDto? categoria = await contexto.Categorias
            .Where(categoria => categoria.Id == id)
            .Select(categoria =>
                new CategoriaDto(categoria.Id, categoria.Nombre))
            .FirstOrDefaultAsync();

        // Si no existe devolvemos 404; en caso contrario devolvemos 200 y JSON.
        return categoria is null
            ? NotFound()
            : Ok(categoria);
    }
}

