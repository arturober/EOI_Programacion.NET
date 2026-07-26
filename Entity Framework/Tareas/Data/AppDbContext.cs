using Microsoft.EntityFrameworkCore;
using Tareas.Models;

namespace Tareas.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Tarea> Tareas => Set<Tarea>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
     base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Tarea>(entity =>
      {
        entity.HasKey(t => t.Id); // Clave primaria
        entity.Property(t => t.Descripcion).IsRequired().HasMaxLength(500);
        entity.Property(t => t.EstaAcabada).HasDefaultValue(false);
      });
  }
}
