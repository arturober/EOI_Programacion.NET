using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;

namespace OpenFoodFacts.Pages.Cuenta;

// Elimina la cookie de autenticación actual.
[Authorize]
public class LogoutModel : PageModel
{
    private readonly SignInManager<Usuario> _signInManager;

    public LogoutModel(SignInManager<Usuario> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        TempData["Mensaje"] = "Has cerrado la sesión.";
        return RedirectToPage("/Index");
    }
}
