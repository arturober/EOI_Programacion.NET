using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Peliculas.Modelos;

// IdentityUser ya aporta correo, contraseña cifrada y datos de seguridad.
public class Usuario : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = "";

    public ICollection<Favorito> Favoritos { get; set; } = [];
}
