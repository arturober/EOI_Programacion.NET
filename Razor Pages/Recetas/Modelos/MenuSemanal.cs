namespace Recetas.Modelos;

// Asigna como máximo una receta a cada día de la semana del usuario.
public class MenuSemanal
{
    public string UsuarioId { get; set; } = "";
    public Usuario Usuario { get; set; } = null!;

    public DiaMenu Dia { get; set; }

    public int RecetaId { get; set; }
    public Receta Receta { get; set; } = null!;

    public DateTime FechaActualizadaUtc { get; set; }
}
