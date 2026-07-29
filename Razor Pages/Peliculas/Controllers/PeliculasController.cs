using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Modelos;
using Peliculas.Servicios;

namespace Peliculas.Controllers;

// Ofrece ejemplos JSON para reutilizar la misma lógica desde otro cliente.
[ApiController]
[Route("api")]
public class PeliculasController : ControllerBase
{
    private readonly ITmdbServicio _tmdb;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public PeliculasController(
        ITmdbServicio tmdb,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _tmdb = tmdb;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    [HttpGet("peliculas/populares")]
    public Task<IActionResult> PopularesAsync(
        [FromQuery] int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        return EjecutarTmdbAsync(
            () => _tmdb.ObtenerListadoAsync(
                TipoListado.Populares, pagina, cancellationToken));
    }

    [HttpGet("peliculas/buscar")]
    public Task<IActionResult> BuscarAsync(
        [FromQuery] string texto,
        [FromQuery] int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < 2)
        {
            return Task.FromResult<IActionResult>(
                BadRequest(new
                {
                    error = "Escribe al menos dos caracteres para buscar."
                }));
        }

        return EjecutarTmdbAsync(
            () => _tmdb.BuscarAsync(
                texto.Trim(), pagina, cancellationToken));
    }

    [HttpGet("peliculas/{id:int}")]
    public Task<IActionResult> DetalleAsync(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return Task.FromResult<IActionResult>(
                BadRequest(new { error = "El identificador no es válido." }));
        }

        return EjecutarTmdbAsync(
            () => _tmdb.ObtenerDetalleAsync(id, cancellationToken));
    }

    [Authorize]
    [HttpGet("favoritos")]
    public async Task<IActionResult> FavoritosAsync(
        CancellationToken cancellationToken)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        IReadOnlyList<Favorito> favoritos = await _favoritos.ListarAsync(
            usuarioId, cancellationToken);

        // No devolvemos datos personales ni objetos de Identity.
        var respuesta = favoritos.Select(favorito => new
        {
            id = favorito.Pelicula.TmdbId,
            titulo = favorito.Pelicula.Titulo,
            tituloOriginal = favorito.Pelicula.TituloOriginal,
            rutaPoster = favorito.Pelicula.RutaPoster,
            fechaEstreno = favorito.Pelicula.FechaEstreno,
            puntuacion = favorito.Pelicula.Puntuacion,
            fechaAgregadaUtc = favorito.FechaAgregadaUtc
        });

        return Ok(respuesta);
    }

    private async Task<IActionResult> EjecutarTmdbAsync<T>(
        Func<Task<T>> operacion)
    {
        try
        {
            T resultado = await operacion();
            return Ok(resultado);
        }
        catch (TmdbExcepcion excepcion)
        {
            int codigo = excepcion.CodigoEstado is null
                ? StatusCodes.Status503ServiceUnavailable
                : (int)excepcion.CodigoEstado.Value;

            return StatusCode(codigo, new { error = excepcion.Message });
        }
    }
}
