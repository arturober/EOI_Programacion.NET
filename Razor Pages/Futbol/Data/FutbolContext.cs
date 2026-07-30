using Futbol.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Futbol.Data;

// Un único contexto contiene las tablas de Identity y los equipos favoritos.
public class FutbolContext : IdentityDbContext<Usuario>
{
    public FutbolContext(DbContextOptions<FutbolContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<EquipoFavorito> EquiposFavoritos => Set<EquipoFavorito>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Un usuario no puede guardar dos veces el mismo equipo.
        modelBuilder.Entity<EquipoFavorito>()
            .HasIndex(favorito => new
            {
                favorito.UsuarioId,
                favorito.EquipoId
            })
            .IsUnique();

        modelBuilder.Entity<EquipoFavorito>()
            .HasOne(favorito => favorito.Usuario)
            .WithMany(usuario => usuario.EquiposFavoritos)
            .HasForeignKey(favorito => favorito.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
