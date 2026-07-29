namespace MazmorraOnline.Dtos;

// Respuesta que recibe quien entra correctamente en el juego.
public class AccesoJuegoRespuesta
{
    // Este identificador relaciona el navegador con su jugador.
    public string JugadorId { get; set; } = "";
}
