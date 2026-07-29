using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Peliculas.Modelos;

namespace Peliculas.Data;

// IdentityDbContext añade al contexto todas las tablas de usuarios y seguridad.
public class PeliculasContext : IdentityDbContext<Usuario>
{
    public PeliculasContext(DbContextOptions<PeliculasContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Pelicula> Peliculas => Set<Pelicula>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity necesita ejecutar primero su propia configuración.
        base.OnModelCreating(modelBuilder);

        // La combinación impide que un usuario repita la misma película.
        modelBuilder.Entity<Favorito>()
            .HasKey(favorito => new
            {
                favorito.UsuarioId,
                favorito.PeliculaId
            });

        modelBuilder.Entity<Favorito>()
            .HasOne(favorito => favorito.Usuario)
            .WithMany(usuario => usuario.Favoritos)
            .HasForeignKey(favorito => favorito.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorito>()
            .HasOne(favorito => favorito.Pelicula)
            .WithMany(pelicula => pelicula.Favoritos)
            .HasForeignKey(favorito => favorito.PeliculaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
