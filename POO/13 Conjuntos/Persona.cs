class Persona(string nombre, int edad)
{
  public string Nombre { get; set; } = nombre;
  public int Edad { get; set; } = edad;

  public override int GetHashCode() =>  Tuple.Create(Nombre, Edad).GetHashCode();

  public override bool Equals(object? obj)
  {
    if(obj == null || obj is not Persona) return false;
    var p = (Persona)obj;
    return p.Nombre == Nombre && p.Edad == Edad;
  }
}
