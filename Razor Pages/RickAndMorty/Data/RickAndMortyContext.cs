using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RickAndMorty.Modelos;

namespace RickAndMorty.Data;

// Un único contexto contiene Identity y la colección de favoritos.
public class RickAndMortyContext : IdentityDbContext<Usuario>
{
    public RickAndMortyContext(
        DbContextOptions<RickAndMortyContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<PersonajeFavorito> PersonajesFavoritos =>
        Set<PersonajeFavorito>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cada personaje solo puede aparecer una vez por usuario.
        modelBuilder.Entity<PersonajeFavorito>()
            .HasIndex(favorito => new
            {
                favorito.UsuarioId,
                favorito.PersonajeId
            })
            .IsUnique();

        modelBuilder.Entity<PersonajeFavorito>()
            .HasOne(favorito => favorito.Usuario)
            .WithMany(usuario => usuario.PersonajesFavoritos)
            .HasForeignKey(favorito => favorito.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
