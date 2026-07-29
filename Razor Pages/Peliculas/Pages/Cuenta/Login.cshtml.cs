using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;

namespace Peliculas.Pages.Cuenta;

public class LoginModel : PageModel
{
    private readonly SignInManager<Usuario> _signInManager;

    public LoginModel(SignInManager<Usuario> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public LoginEntrada Entrada { get; set; } = new();

    public string? ReturnUrl { get; private set; }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Microsoft.AspNetCore.Identity.SignInResult resultado =
            await _signInManager.PasswordSignInAsync(
                Entrada.Email.Trim(),
                Entrada.Contrasena,
                Entrada.Recordarme,
                lockoutOnFailure: true);

        if (resultado.Succeeded)
        {
            TempData["Mensaje"] = "Has iniciado sesión correctamente.";

            return Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl!)
                : RedirectToPage("/Index");
        }

        if (resultado.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "La cuenta está bloqueada durante cinco minutos.");
        }
        else
        {
            // El mensaje no revela si el correo está registrado.
            ModelState.AddModelError(
                string.Empty,
                "El correo o la contraseña no son correctos.");
        }

        return Page();
    }

    public class LoginEntrada
    {
        [Required(ErrorMessage = "Escribe tu correo electrónico.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Escribe tu contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = "";

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }
    }
}
