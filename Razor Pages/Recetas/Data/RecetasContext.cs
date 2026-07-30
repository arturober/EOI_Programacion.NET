using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Recetas.Modelos;

namespace Recetas.Data;

// Un único contexto administra las tablas de Identity y las de la aplicación.
public class RecetasContext : IdentityDbContext<Usuario>
{
    public RecetasContext(DbContextOptions<RecetasContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();
    public DbSet<MenuSemanal> MenusSemanales => Set<MenuSemanal>();

    protected override void OnModelCreating(ModelBuilder constructor)
    {
        base.OnModelCreating(constructor);

        constructor.Entity<Favorito>()
            .HasKey(elemento => new
            {
                elemento.UsuarioId,
                elemento.RecetaId
            });

        constructor.Entity<MenuSemanal>()
            .HasKey(elemento => new
            {
                elemento.UsuarioId,
                elemento.Dia
            });

        constructor.Entity<Favorito>()
            .HasOne(elemento => elemento.Usuario)
            .WithMany(usuario => usuario.Favoritos)
            .HasForeignKey(elemento => elemento.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.Entity<Favorito>()
            .HasOne(elemento => elemento.Receta)
            .WithMany(receta => receta.Favoritos)
            .HasForeignKey(elemento => elemento.RecetaId)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.Entity<MenuSemanal>()
            .HasOne(elemento => elemento.Usuario)
            .WithMany(usuario => usuario.MenuSemanal)
            .HasForeignKey(elemento => elemento.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.Entity<MenuSemanal>()
            .HasOne(elemento => elemento.Receta)
            .WithMany(receta => receta.DiasMenu)
            .HasForeignKey(elemento => elemento.RecetaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
