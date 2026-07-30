using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Recetas.Modelos;

// Amplía el usuario de Identity con un nombre visible.
public class Usuario : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = "";

    public ICollection<Favorito> Favoritos { get; set; } = [];
    public ICollection<MenuSemanal> MenuSemanal { get; set; } = [];
}
