using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenFoodFacts.Modelos;

namespace OpenFoodFacts.Data;

// Un único contexto administra las tablas de Identity y las de la aplicación.
public class AlimentosContext : IdentityDbContext<Usuario>
{
    public AlimentosContext(DbContextOptions<AlimentosContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<ProductoGuardado> Productos => Set<ProductoGuardado>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();

    protected override void OnModelCreating(ModelBuilder constructor)
    {
        base.OnModelCreating(constructor);

        constructor.Entity<Favorito>()
            .HasKey(elemento => new
            {
                elemento.UsuarioId,
                elemento.ProductoCodigo
            });

        constructor.Entity<Favorito>()
            .HasOne(elemento => elemento.Usuario)
            .WithMany(usuario => usuario.Favoritos)
            .HasForeignKey(elemento => elemento.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.Entity<Favorito>()
            .HasOne(elemento => elemento.Producto)
            .WithMany(producto => producto.Favoritos)
            .HasForeignKey(elemento => elemento.ProductoCodigo)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
