namespace MazmorraOnline.Dtos;

// Versión pública de un mapa, sin las listas internas para la física.
public class MapaDto
{
    public string Nombre { get; set; } = "";
    public List<string> Filas { get; set; } = new();
}
