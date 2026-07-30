using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Modelos;

// Identity aporta correo, contraseña protegida y datos de seguridad.
public class Usuario : IdentityUser
{
    public string Nombre { get; set; } = "";

    public ICollection<Favorito> Favoritos { get; set; } = [];
}
