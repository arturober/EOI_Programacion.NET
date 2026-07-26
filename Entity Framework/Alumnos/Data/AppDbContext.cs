using Alumnos.Models;
using Microsoft.EntityFrameworkCore;

namespace Alumnos.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Alumno> Alumnos => Set<Alumno>();
    public DbSet<Asignatura> Asignaturas => Set<Asignatura>();
}
