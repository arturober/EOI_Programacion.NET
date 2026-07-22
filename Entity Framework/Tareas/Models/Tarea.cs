using System.ComponentModel.DataAnnotations.Schema;

namespace Tareas.Models;

public class Tarea
{
  public int Id { get; set; }

  public required string Descripcion { get; set; }

  public bool EstaAcabada { get; set; }

  public DateTime? Fecha { get; set; }
}
