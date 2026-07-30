using NasaExplorer.Modelos;

namespace NasaExplorer.Servicios;

// Define las operaciones de la colección privada de cada usuario.
public interface IFavoritosServicio
{
    Task<List<Favorito>> ObtenerAsync(string usuarioId);
    Task<HashSet<string>> ObtenerReferenciasAsync(string usuarioId, string tipo);
    Task<bool> AlternarAsync(string usuarioId, Favorito favorito);
    Task<bool> EliminarAsync(string usuarioId, int id);
}
