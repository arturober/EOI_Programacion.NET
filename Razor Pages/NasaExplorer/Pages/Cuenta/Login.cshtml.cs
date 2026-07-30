using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.Modelos;

namespace NasaExplorer.Pages.Cuenta;

// Identity comprueba la contraseña cifrada y crea la cookie de sesión.
public class LoginModel(SignInManager<Usuario> signInManager) : PageModel
{
    [BindProperty]
    public EntradaLogin Entrada { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // El tipo completo evita la ambigüedad con Mvc.SignInResult.
        Microsoft.AspNetCore.Identity.SignInResult resultado =
            await signInManager.PasswordSignInAsync(
                Entrada.Email,
                Entrada.Contrasena,
                Entrada.Recordarme,
                lockoutOnFailure: true);

        if (resultado.Succeeded)
        {
            TempData["Mensaje"] = "Has iniciado sesión correctamente.";
            return Url.IsLocalUrl(ReturnUrl)
                ? LocalRedirect(ReturnUrl!)
                : RedirectToPage("/Index");
        }

        ModelState.AddModelError(
            string.Empty,
            resultado.IsLockedOut
                ? "La cuenta está bloqueada temporalmente."
                : "El correo o la contraseña no son correctos.");
        return Page();
    }

    // Este modelo contiene exclusivamente los campos visibles del formulario.
    public class EntradaLogin
    {
        [Required(ErrorMessage = "Escribe tu correo.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe tu contraseña.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }
    }
}
