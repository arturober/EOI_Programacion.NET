using Microsoft.EntityFrameworkCore;
using TrivialApi.Models;

namespace TrivialApi.Data;

// El contexto representa la conexión de Entity Framework con la base de datos SQLite.
// El constructor primario recibe las opciones registradas anteriormente en Program.cs.
public class TrivialContext(DbContextOptions<TrivialContext> opciones)
    : DbContext(opciones)
{
    // Cada DbSet representa una tabla y es el punto de partida de sus consultas.
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Creamos un índice único para impedir dos categorías con el mismo nombre.
        // La restricción se aplica en la propia base de datos, no solo en el formulario.
        modelBuilder.Entity<Categoria>()
            .HasIndex(categoria => categoria.Nombre)
            .IsUnique();

        // Configuramos de forma explícita la relación uno a muchos.
        // Una pregunta tiene una categoría y una categoría contiene muchas preguntas.
        modelBuilder.Entity<Pregunta>()
            .HasOne(pregunta => pregunta.Categoria)
            .WithMany(categoria => categoria.Preguntas)
            .HasForeignKey(pregunta => pregunta.CategoriaId)
            .OnDelete(DeleteBehavior.Cascade);

        // El borrado en cascada significa que, al borrar una categoría, SQLite
        // elimina también las preguntas que dependían de ella.
    }
}

