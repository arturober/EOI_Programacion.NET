class Persona
{
  public string Nombre { get; set; }
  public int Edad { get; set => field = value < 0 ? field : value; }

  public string[] Telefonos { get; set; } = [];

  public Direccion? Direccion { get; set; }

  public Persona(string nombre, int edad)
  {
    Nombre = nombre;
    Edad = edad;
  }

  // Constructor de copia
  public Persona(Persona p)
  {
    Nombre = p.Nombre;
    Edad = p.Edad;
    Direccion = p.Direccion != null ? new Direccion(p.Direccion) : null;
    Telefonos = (string[])p.Telefonos.Clone();
  }

  public void Saluda()
  {
    Console.WriteLine($"{Nombre} tiene {Edad} años");
    if (Direccion != null)
    {
      Console.WriteLine($"Vivo en la calle {Direccion.Calle}, número {Direccion.Numero} ({Direccion.CP})");
    }
    Console.WriteLine($"Mis teléfonos son: {String.Join(", ", Telefonos)}");
  }
}
