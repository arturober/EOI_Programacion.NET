using Microsoft.AspNetCore.Identity;

namespace NasaExplorer.Servicios;

// Traduce los mensajes habituales de Identity al español.
public class ErroresIdentityEnEspanol : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email) =>
        Error(nameof(DuplicateEmail), $"El correo «{email}» ya está registrado.");

    public override IdentityError DuplicateUserName(string userName) =>
        Error(nameof(DuplicateUserName), $"El usuario «{userName}» ya existe.");

    public override IdentityError InvalidEmail(string? email) =>
        Error(nameof(InvalidEmail), $"El correo «{email}» no es válido.");

    public override IdentityError PasswordTooShort(int length) =>
        Error(nameof(PasswordTooShort), $"La contraseña debe tener al menos {length} caracteres.");

    public override IdentityError PasswordRequiresDigit() =>
        Error(nameof(PasswordRequiresDigit), "La contraseña debe incluir un número.");

    public override IdentityError PasswordRequiresLower() =>
        Error(nameof(PasswordRequiresLower), "La contraseña debe incluir una minúscula.");

    public override IdentityError PasswordRequiresUpper() =>
        Error(nameof(PasswordRequiresUpper), "La contraseña debe incluir una mayúscula.");

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        Error(nameof(PasswordRequiresNonAlphanumeric), "La contraseña debe incluir un símbolo.");

    private static IdentityError Error(string codigo, string descripcion) =>
        new() { Code = codigo, Description = descripcion };
}
