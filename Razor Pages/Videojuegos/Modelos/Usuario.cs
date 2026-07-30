using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Videojuegos.Modelos;

// Amplía el usuario de Identity con un nombre visible.
public class Usuario : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = "";

    public ICollection<VideojuegoUsuario> Biblioteca { get; set; } = [];
}
