using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Servicios;

// Sustituye los mensajes ingleses de Identity por textos claros.
public class ErroresIdentityEnEspanol : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email) =>
        Crear("DuplicateEmail", "Ya existe una cuenta con ese correo.");

    public override IdentityError DuplicateUserName(string userName) =>
        Crear("DuplicateUserName", "Ya existe una cuenta con ese correo.");

    public override IdentityError InvalidEmail(string? email) =>
        Crear("InvalidEmail", "El correo electrónico no es válido.");

    public override IdentityError InvalidUserName(string? userName) =>
        Crear("InvalidUserName", "El correo electrónico no es válido.");

    public override IdentityError PasswordTooShort(int length) =>
        Crear(
            "PasswordTooShort",
            $"La contraseña debe tener al menos {length} caracteres.");

    public override IdentityError PasswordRequiresDigit() =>
        Crear(
            "PasswordRequiresDigit",
            "La contraseña debe incluir al menos un número.");

    public override IdentityError PasswordRequiresLower() =>
        Crear(
            "PasswordRequiresLower",
            "La contraseña debe incluir al menos una minúscula.");

    public override IdentityError PasswordRequiresUpper() =>
        Crear(
            "PasswordRequiresUpper",
            "La contraseña debe incluir al menos una mayúscula.");

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        Crear(
            "PasswordRequiresNonAlphanumeric",
            "La contraseña debe incluir un carácter especial.");

    private static IdentityError Crear(
        string codigo,
        string descripcion)
    {
        return new IdentityError
        {
            Code = codigo,
            Description = descripcion
        };
    }
}
