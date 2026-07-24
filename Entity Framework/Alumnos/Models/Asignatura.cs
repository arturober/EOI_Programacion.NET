namespace Alumnos.Models;

public class Asignatura
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nombre { get; set; }
    public required string Codigo { get; set; }
    public int Creditos { get; set; }

    // Relación Muchos a Muchos implícita en EF Core
    public List<Alumno> Alumnos { get; set; } = [];
}
