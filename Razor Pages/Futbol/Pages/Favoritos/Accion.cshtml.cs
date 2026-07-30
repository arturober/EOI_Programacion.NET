using System.ComponentModel.DataAnnotations;
using Futbol.Modelos;
using Futbol.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Futbol.Pages.Favoritos;

// Recibe formularios POST para añadir o quitar equipos.
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
    public int EquipoId { get; set; }

    [BindProperty, Required, StringLength(150)]
    public string Nombre { get; set; } = "";

    [BindProperty, StringLength(100)]
    public string NombreCorto { get; set; } = "";

    [BindProperty, StringLength(500)]
    public string? EscudoUrl { get; set; }

    [BindProperty, StringLength(150)]
    public string? Competicion { get; set; }

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        // Esta página solo acepta cambios mediante POST.
        return RedirectToPage("/Favoritos/Index");
    }

    public async Task<IActionResult> OnPostAgregarAsync(
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "No se ha podido identificar correctamente el equipo.";
            return Volver();
        }

        string usuarioId = _userManager.GetUserId(User)!;

        bool agregado = await _favoritos.AgregarAsync(
            usuarioId,
            new EquipoFavorito
            {
                EquipoId = EquipoId,
                Nombre = Nombre.Trim(),
                NombreCorto = NombreCorto.Trim(),
                EscudoUrl = EscudoUrl,
                Competicion = Competicion
            },
            cancellationToken);

        TempData["Mensaje"] = agregado
            ? "Equipo añadido a tus favoritos."
            : "Ese equipo ya estaba en tus favoritos.";

        return Volver();
    }

    public async Task<IActionResult> OnPostEliminarAsync(
        CancellationToken cancellationToken)
    {
        string usuarioId = _userManager.GetUserId(User)!;
        bool eliminado = await _favoritos.EliminarAsync(
            usuarioId,
            EquipoId,
            cancellationToken);

        TempData["Mensaje"] = eliminado
            ? "Equipo eliminado de tus favoritos."
            : "El equipo ya no estaba en tus favoritos.";

        return Volver();
    }

    private IActionResult Volver()
    {
        return Url.IsLocalUrl(ReturnUrl)
            ? LocalRedirect(ReturnUrl!)
            : RedirectToPage("/Favoritos/Index");
    }
}
