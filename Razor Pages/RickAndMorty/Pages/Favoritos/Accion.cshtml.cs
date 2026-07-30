using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.Modelos;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Favoritos;

// Recibe formularios POST para añadir o quitar personajes.
[Authorize]
public class AccionModel : PageModel
{
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public AccionModel(
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _favoritos = favoritos;
        _userManager = userManager;
    }

    [BindProperty, Range(1, int.MaxValue)]
    public int PersonajeId { get; set; }

    [BindProperty, Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [BindProperty, StringLength(50)]
    public string Estado { get; set; } = "";

    [BindProperty, StringLength(100)]
    public string Especie { get; set; } = "";

    [BindProperty, StringLength(500)]
    public string ImagenUrl { get; set; } = "";

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Favoritos/Index");
    }

    public async Task<IActionResult> OnPostAgregarAsync(
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "No se ha podido identificar correctamente el personaje.";
            return Volver();
        }

        string usuarioId = _userManager.GetUserId(User)!;
        bool agregado = await _favoritos.AgregarAsync(
            usuarioId,
            new PersonajeFavorito
            {
                PersonajeId = PersonajeId,
                Nombre = Nombre.Trim(),
                Estado = Estado.Trim(),
                Especie = Especie.Trim(),
                ImagenUrl = ImagenUrl.Trim()
            },
            cancellationToken);

        TempData["Mensaje"] = agregado
            ? "Personaje añadido a tus favoritos."
            : "Ese personaje ya estaba en tus favoritos.";

        return Volver();
    }

    public async Task<IActionResult> OnPostEliminarAsync(
        CancellationToken cancellationToken)
    {
        string usuarioId = _userManager.GetUserId(User)!;
        bool eliminado = await _favoritos.EliminarAsync(
            usuarioId,
            PersonajeId,
            cancellationToken);

        TempData["Mensaje"] = eliminado
            ? "Personaje eliminado de tus favoritos."
            : "El personaje ya no estaba en tus favoritos.";

        return Volver();
    }

    private IActionResult Volver()
    {
        return Url.IsLocalUrl(ReturnUrl)
            ? LocalRedirect(ReturnUrl!)
            : RedirectToPage("/Favoritos/Index");
    }
}
