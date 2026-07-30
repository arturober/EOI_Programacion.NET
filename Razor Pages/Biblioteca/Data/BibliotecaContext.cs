using Biblioteca.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Data;

// Un único contexto administra las tablas de Identity y las de la aplicación.
public class BibliotecaContext : IdentityDbContext<Usuario>
{
    public BibliotecaContext(DbContextOptions<BibliotecaContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();

    protected override void OnModelCreating(ModelBuilder constructor)
    {
        base.OnModelCreating(constructor);

        // Un usuario solo puede guardar una vez la misma obra.
        constructor.Entity<Favorito>()
            .HasKey(favorito => new
            {
                favorito.UsuarioId,
                favorito.LibroId
            });

        constructor.Entity<Favorito>()
            .HasOne(favorito => favorito.Usuario)
            .WithMany(usuario => usuario.Favoritos)
            .HasForeignKey(favorito => favorito.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.Entity<Favorito>()
            .HasOne(favorito => favorito.Libro)
            .WithMany(libro => libro.Favoritos)
            .HasForeignKey(favorito => favorito.LibroId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
