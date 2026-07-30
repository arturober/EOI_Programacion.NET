using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.Modelos;

namespace NasaExplorer.Pages.Cuenta;

// Cerrar sesión elimina la cookie de autenticación de forma segura.
public class SalirModel(SignInManager<Usuario> signInManager) : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await signInManager.SignOutAsync();
        TempData["Mensaje"] = "Has cerrado la sesión.";
        return RedirectToPage("/Index");
    }
}
