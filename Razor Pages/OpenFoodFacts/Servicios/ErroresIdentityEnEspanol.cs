using Microsoft.AspNetCore.Identity;

namespace OpenFoodFacts.Servicios;

// Traduce los errores más habituales de Identity al español.
public class ErroresIdentityEnEspanol : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email) =>
        Crear("CorreoDuplicado", "Ya existe una cuenta con ese correo.");

    public override IdentityError DuplicateUserName(string userName) =>
        Crear("UsuarioDuplicado", "Ya existe una cuenta con ese correo.");

    public override IdentityError InvalidEmail(string? email) =>
        Crear("CorreoNoValido", "El correo electrónico no es válido.");

    public override IdentityError PasswordTooShort(int length) =>
        Crear(
            "ContrasenaCorta",
            $"La contraseña debe tener al menos {length} caracteres.");

    public override IdentityError PasswordRequiresDigit() =>
        Crear(
            "ContrasenaSinNumero",
            "La contraseña debe contener al menos un número.");

    public override IdentityError PasswordRequiresLower() =>
        Crear(
            "ContrasenaSinMinuscula",
            "La contraseña debe contener al menos una minúscula.");

    public override IdentityError PasswordRequiresUpper() =>
        Crear(
            "ContrasenaSinMayuscula",
            "La contraseña debe contener al menos una mayúscula.");

    private static IdentityError Crear(string codigo, string descripcion) =>
        new()
        {
            Code = codigo,
            Description = descripcion
        };
}
