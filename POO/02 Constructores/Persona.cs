class Persona(string nombre, int edad)
{
  public string? Nombre { get; set; } = nombre;
  public int Edad { get; set; } = edad;
  public string? DNI { get; set; }
  public DateOnly? FechaNac { get; set; }
}
