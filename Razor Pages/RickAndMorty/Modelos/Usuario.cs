using Microsoft.AspNetCore.Identity;

namespace RickAndMorty.Modelos;

// Amplía el usuario de Identity con un nombre visible.
public class Usuario : IdentityUser
{
    public string Nombre { get; set; } = "";

    public ICollection<PersonajeFavorito> PersonajesFavoritos { get; set; } =
        new List<PersonajeFavorito>();
}
