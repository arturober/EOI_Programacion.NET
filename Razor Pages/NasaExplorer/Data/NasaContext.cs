using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NasaExplorer.Modelos;

namespace NasaExplorer.Data;

// Un único contexto contiene las tablas de Identity y la tabla de favoritos.
public class NasaContext(DbContextOptions<NasaContext> options)
    : IdentityDbContext<Usuario>(options)
{
    public DbSet<Favorito> Favoritos => Set<Favorito>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Un usuario no puede guardar dos veces el mismo elemento del mismo tipo.
        builder.Entity<Favorito>()
            .HasIndex(favorito => new
            {
                favorito.UsuarioId,
                favorito.Tipo,
                favorito.Referencia
            })
            .IsUnique();

        // Al borrar un usuario también desaparecen sus favoritos.
        builder.Entity<Favorito>()
            .HasOne(favorito => favorito.Usuario)
            .WithMany()
            .HasForeignKey(favorito => favorito.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
