using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers;

// Expone una pequeña API propia para practicar también respuestas JSON.
[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
    private readonly IOpenLibraryServicio _openLibrary;

    public LibrosController(IOpenLibraryServicio openLibrary)
    {
        _openLibrary = openLibrary;
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<PaginaLibros>> Buscar(
        [FromQuery] string texto,
        [FromQuery] int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < 2)
        {
            return BadRequest(new
            {
                mensaje = "El texto debe contener al menos dos caracteres."
            });
        }

        try
        {
            PaginaLibros resultado = await _openLibrary.BuscarAsync(
                texto,
                pagina,
                cancellationToken);

            return Ok(resultado);
        }
        catch (OpenLibraryExcepcion excepcion)
        {
            return StatusCode(
                excepcion.CodigoEstado is null
                    ? StatusCodes.Status503ServiceUnavailable
                    : (int)excepcion.CodigoEstado.Value,
                new { mensaje = excepcion.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LibroDetalle>> Detalle(
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            LibroDetalle libro = await _openLibrary.ObtenerDetalleAsync(
                id,
                cancellationToken);

            return Ok(libro);
        }
        catch (OpenLibraryExcepcion excepcion)
        {
            return StatusCode(
                excepcion.CodigoEstado is null
                    ? StatusCodes.Status503ServiceUnavailable
                    : (int)excepcion.CodigoEstado.Value,
                new { mensaje = excepcion.Message });
        }
    }
}
