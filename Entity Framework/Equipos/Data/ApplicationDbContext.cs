using Equipos.Models;
using Microsoft.EntityFrameworkCore;

namespace Equipos.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
  public DbSet<Equipo> Equipos => Set<Equipo>();
  public DbSet<Jugador> Jugadores => Set<Jugador>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Las claves primarias y relaciones deberían configurarse automáticamente
    modelBuilder.Entity<Equipo>(entity =>
    {
      entity.Property(e => e.Nombre).HasMaxLength(100);
      entity.HasMany(e => e.Jugadores)
        .WithOne(j => j.Equipo)
        .HasForeignKey(j => j.EquipoId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Jugador>(entity =>
    {
      entity.Property(e => e.Nombre).HasMaxLength(100);
    });

    // Seeding (inserción inicial de valores)
    modelBuilder.Entity<Equipo>().HasData(
      new Equipo { Id = 1, Nombre = "Perdedores SA" },
      new Equipo { Id = 2, Nombre = "Ganadores Unidos" }
    );

    modelBuilder.Entity<Jugador>().HasData(
      new Jugador { Id = 1, Nombre = "Pepito Sánchez", EquipoId = 1 },
      new Jugador { Id = 2, Nombre = "Juan Pérez", EquipoId = 1 },
      new Jugador { Id = 3, Nombre = "Benito Fernández", EquipoId = 2 }
    );
  }
}
