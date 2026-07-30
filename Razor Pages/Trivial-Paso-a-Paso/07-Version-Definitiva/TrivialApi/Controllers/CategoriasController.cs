using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrivialApi.Data;
using TrivialApi.DTOs;

namespace TrivialApi.Controllers;

[ApiController]

[Route("api/categorias")]
public class CategoriasController(TrivialContext contexto) : ControllerBase
{
    [HttpGet]
    public async Task<List<CategoriaDto>> ObtenerTodas()
    {
        return await contexto.Categorias
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaDto(c.Id, c.Nombre))
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> ObtenerPorId(int id)
    {
        CategoriaDto? categoria = await contexto.Categorias
            .Where(c => c.Id == id)
            .Select(c => new CategoriaDto(c.Id, c.Nombre))
            .FirstOrDefaultAsync();

        return categoria is not null ? Ok(categoria) : NotFound();
    }
}