using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Videojuegos.Modelos;

namespace Videojuegos.Data;

// Un único contexto administra las tablas de Identity y las de la aplicación.
public class VideojuegosContext : IdentityDbContext<Usuario>
{
    public VideojuegosContext(DbContextOptions<VideojuegosContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Videojuego> Videojuegos => Set<Videojuego>();
    public DbSet<VideojuegoUsuario> Bibliotecas => Set<VideojuegoUsuario>();

    protected override void OnModelCreating(ModelBuilder constructor)
    {
        base.OnModelCreating(constructor);

        // Un usuario solo puede guardar una vez el mismo videojuego.
        constructor.Entity<VideojuegoUsuario>()
            .HasKey(elemento => new
            {
                elemento.UsuarioId,
                elemento.VideojuegoId
            });

        constructor.Entity<VideojuegoUsuario>()
            .HasOne(elemento => elemento.Usuario)
            .WithMany(usuario => usuario.Biblioteca)
            .HasForeignKey(elemento => elemento.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.Entity<VideojuegoUsuario>()
            .HasOne(elemento => elemento.Videojuego)
            .WithMany(videojuego => videojuego.Usuarios)
            .HasForeignKey(elemento => elemento.VideojuegoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
