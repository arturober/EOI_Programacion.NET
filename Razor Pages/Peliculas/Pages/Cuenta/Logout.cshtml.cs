using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;

namespace Peliculas.Pages.Cuenta;

[Authorize]
public class LogoutModel : PageModel
{
    private readonly SignInManager<Usuario> _signInManager;

    public LogoutModel(SignInManager<Usuario> signInManager)
    {
        _signInManager = signInManager;
    }

    // Cerrar sesión modifica la cookie, por eso se realiza mediante POST.
    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        TempData["Mensaje"] = "Has cerrado la sesión.";
        return RedirectToPage("/Index");
    }
}
