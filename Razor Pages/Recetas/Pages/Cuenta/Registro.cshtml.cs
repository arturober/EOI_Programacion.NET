using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;

namespace Recetas.Pages.Cuenta;

// Gestiona el formulario de alta de usuarios.
public class RegistroModel : PageModel
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;

    public RegistroModel(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public RegistroEntrada Entrada { get; set; } = new();

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

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        string email = Entrada.Email.Trim();
        Usuario usuario = new()
        {
            Nombre = Entrada.Nombre.Trim(),
            UserName = email,
            Email = email
        };

        IdentityResult resultado =
            await _userManager.CreateAsync(usuario, Entrada.Contrasena);

        if (!resultado.Succeeded)
        {
            foreach (IdentityError error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        // No se exige confirmar el correo y la sesión comienza al registrarse.
        await _signInManager.SignInAsync(usuario, isPersistent: false);

        TempData["Mensaje"] =
            $"Bienvenido, {usuario.Nombre}. Tu cuenta ya está preparada.";

        return Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl!)
            : RedirectToPage("/Index");
    }

    public class RegistroEntrada
    {
        [Required(ErrorMessage = "Escribe tu nombre.")]
        [StringLength(
            50,
            MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "Escribe tu correo electrónico.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(254)]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Escribe una contraseña.")]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = "";

        [Required(ErrorMessage = "Repite la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Contrasena),
            ErrorMessage = "Las dos contraseñas no coinciden.")]
        [Display(Name = "Repetir contraseña")]
        public string ConfirmacionContrasena { get; set; } = "";
    }
}
