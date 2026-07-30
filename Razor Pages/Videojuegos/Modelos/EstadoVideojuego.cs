using System.ComponentModel.DataAnnotations;

namespace Videojuegos.Modelos;

// Representa el progreso personal del usuario con cada videojuego.
public enum EstadoVideojuego
{
    [Display(Name = "Pendiente")]
    Pendiente,

    [Display(Name = "Jugando")]
    Jugando,

    [Display(Name = "Completado")]
    Completado,

    [Display(Name = "Abandonado")]
    Abandonado
}

public static class EstadoVideojuegoExtensiones
{
    public static string Titulo(this EstadoVideojuego estado) => estado switch
    {
        EstadoVideojuego.Pendiente => "Pendiente",
        EstadoVideojuego.Jugando => "Jugando",
        EstadoVideojuego.Completado => "Completado",
        EstadoVideojuego.Abandonado => "Abandonado",
        _ => "Sin estado"
    };

    public static string ColorBootstrap(this EstadoVideojuego estado) =>
        estado switch
        {
            EstadoVideojuego.Jugando => "primary",
            EstadoVideojuego.Completado => "success",
            EstadoVideojuego.Abandonado => "secondary",
            _ => "warning"
        };
}
