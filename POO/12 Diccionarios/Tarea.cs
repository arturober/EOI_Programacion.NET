class Tarea(string descripcion)
{
  public string Descripcion { get; set; } = descripcion;
  public bool Acabada { get; set; } = false;

  public override string ToString()
  {
    return Descripcion + " (" + (Acabada ? "finalizada" : "pendiente") + ")";
  }
}
