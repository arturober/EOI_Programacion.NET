using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OpenFoodFacts.Modelos;

// Amplía el usuario de Identity con el nombre que se muestra en la interfaz.
public class Usuario : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = "";

    public ICollection<Favorito> Favoritos { get; set; } = [];
}
