namespace Alumnos.Models;

public class Alumno
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nombre { get; set; }
    public required string Email { get; set; }
    public required string Dni { get; set; }

    // Relación Muchos a Muchos implícita en EF Core
    public List<Asignatura> Asignaturas { get; set; } = [];
}
