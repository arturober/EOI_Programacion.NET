using Microsoft.EntityFrameworkCore;
using TrivialApi.Models;

namespace TrivialApi.Data;

public class TrivialContext(DbContextOptions<TrivialContext> opciones): DbContext(opciones)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>()
            .HasIndex(c => c.Nombre)
            .IsUnique();
        modelBuilder.Entity<Pregunta>()
            .HasOne(pregunta => pregunta.Categoria)
            .WithMany(categoria => categoria.Preguntas)
            .HasForeignKey(pregunta => pregunta.CategoriaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}