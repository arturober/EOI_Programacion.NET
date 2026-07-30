using Microsoft.EntityFrameworkCore;
using OpenFoodFacts.Data;
using OpenFoodFacts.Modelos;

namespace OpenFoodFacts.Servicios;

// Mantiene toda la lógica de favoritos fuera de las Razor Pages.
public class ColeccionServicio : IColeccionServicio
{
    private readonly AlimentosContext _contexto;

    public ColeccionServicio(AlimentosContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<HashSet<string>> ObtenerCodigosFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        List<string> codigos = await _contexto.Favoritos
            .AsNoTracking()
            .Where(elemento => elemento.UsuarioId == usuarioId)
            .Select(elemento => elemento.ProductoCodigo)
            .ToListAsync(cancellationToken);

        return codigos.ToHashSet(StringComparer.Ordinal);
    }

    public async Task<IReadOnlyList<Favorito>> ListarFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.Favoritos
            .AsNoTracking()
            .Include(elemento => elemento.Producto)
            .Where(elemento => elemento.UsuarioId == usuarioId)
            .OrderByDescending(elemento => elemento.FechaAgregadoUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarFavoritoAsync(
        string usuarioId,
        ProductoDetalle producto,
        CancellationToken cancellationToken = default)
    {
        bool yaExiste = await _contexto.Favoritos.AnyAsync(
            elemento =>
                elemento.UsuarioId == usuarioId
                && elemento.ProductoCodigo == producto.Codigo,
            cancellationToken);

        if (yaExiste)
        {
            return;
        }

        await GuardarProductoAsync(producto, cancellationToken);

        _contexto.Favoritos.Add(new Favorito
        {
            UsuarioId = usuarioId,
            ProductoCodigo = producto.Codigo,
            FechaAgregadoUtc = DateTime.UtcNow
        });

        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task QuitarFavoritoAsync(
        string usuarioId,
        string codigo,
        CancellationToken cancellationToken = default)
    {
        Favorito? favorito = await _contexto.Favoritos.FindAsync(
            [usuarioId, codigo],
            cancellationToken);

        if (favorito is null)
        {
            return;
        }

        _contexto.Favoritos.Remove(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductoGuardado>>
        ObtenerParaCompararAsync(
            string usuarioId,
            IEnumerable<string> codigos,
            CancellationToken cancellationToken = default)
    {
        string[] seleccion = codigos
            .Where(codigo => !string.IsNullOrWhiteSpace(codigo))
            .Distinct(StringComparer.Ordinal)
            .Take(4)
            .ToArray();

        if (seleccion.Length == 0)
        {
            return [];
        }

        return await _contexto.Favoritos
            .AsNoTracking()
            .Include(elemento => elemento.Producto)
            .Where(elemento =>
                elemento.UsuarioId == usuarioId
                && seleccion.Contains(elemento.ProductoCodigo))
            .Select(elemento => elemento.Producto)
            .OrderBy(producto => producto.Nombre)
            .ToListAsync(cancellationToken);
    }

    private async Task GuardarProductoAsync(
        ProductoDetalle producto,
        CancellationToken cancellationToken)
    {
        ProductoGuardado? guardado =
            await _contexto.Productos.FindAsync(
                [producto.Codigo],
                cancellationToken);

        if (guardado is null)
        {
            guardado = new ProductoGuardado
            {
                Codigo = producto.Codigo
            };

            _contexto.Productos.Add(guardado);
        }

        guardado.Nombre = producto.Nombre;
        guardado.Marca = producto.Marca;
        guardado.ImagenUrl = producto.ImagenUrl;
        guardado.Cantidad = producto.Cantidad;
        guardado.NutriScore = producto.NutriScore;
        guardado.GrupoNova = producto.GrupoNova;
        guardado.GreenScore = producto.GreenScore;
        guardado.EnergiaKcal100g = producto.EnergiaKcal100g;
        guardado.Grasas100g = producto.Grasas100g;
        guardado.GrasasSaturadas100g =
            producto.GrasasSaturadas100g;
        guardado.Hidratos100g = producto.Hidratos100g;
        guardado.Azucares100g = producto.Azucares100g;
        guardado.Fibra100g = producto.Fibra100g;
        guardado.Proteinas100g = producto.Proteinas100g;
        guardado.Sal100g = producto.Sal100g;
        guardado.ActualizadoUtc = DateTime.UtcNow;
    }
}
