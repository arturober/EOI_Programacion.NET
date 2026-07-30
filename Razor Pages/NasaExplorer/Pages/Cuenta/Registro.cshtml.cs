using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.Modelos;

namespace NasaExplorer.Pages.Cuenta;

// El registro crea el usuario y abre su sesión sin confirmar el correo.
public class RegistroModel(
    UserManager<Usuario> userManager,
    SignInManager<Usuario> signInManager) : PageModel
{
    [BindProperty]
    public EntradaRegistro Entrada { get; set; } = new();

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

        Usuario usuario = new()
        {
            UserName = Entrada.Email,
            Email = Entrada.Email
        };
        IdentityResult resultado = await userManager.CreateAsync(
            usuario,
            Entrada.Contrasena);

        if (resultado.Succeeded)
        {
            await signInManager.SignInAsync(usuario, isPersistent: false);
            TempData["Mensaje"] = "Tu cuenta está lista. No necesitas confirmar el correo.";
            return Url.IsLocalUrl(ReturnUrl)
                ? LocalRedirect(ReturnUrl!)
                : RedirectToPage("/Index");
        }

        foreach (IdentityError error in resultado.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }

    public class EntradaRegistro
    {
        [Required(ErrorMessage = "Escribe tu correo.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe una contraseña.")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repite la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Contrasena),
            ErrorMessage = "Las dos contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
