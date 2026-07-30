using Microsoft.AspNetCore.Identity;

namespace Futbol.Modelos;

// Amplía el usuario de Identity con el nombre que se muestra en pantalla.
public class Usuario : IdentityUser
{
    public string Nombre { get; set; } = "";

    public ICollection<EquipoFavorito> EquiposFavoritos { get; set; } =
        new List<EquipoFavorito>();
}
