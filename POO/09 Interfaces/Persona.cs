class Persona: ICloneable, IHablador
{
  public static int NumPersonas { get; private set; } = 0;
  public string Nombre { get; set; }
  public int Edad { get; set => field = value < 0 ? field : value; }

  public string[] Telefonos { get; set; } = [];

  public Direccion? Direccion { get; set; }

  public Persona(string nombre, int edad)
  {
    Nombre = nombre;
    Edad = edad;
    NumPersonas++;
  }

  public Persona(Persona p)
  {
    Nombre = p.Nombre;
    Edad = p.Edad;
    Direccion = p.Direccion != null ? new Direccion(p.Direccion) : null;
    Telefonos = (string[])p.Telefonos.Clone();
    NumPersonas++;
  }

  public static Persona PersonaGenerica()
  {
    return new Persona("Anónimo", 20);
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

  public bool EstoyEnArray(Persona[] personas)
  {
    return Array.IndexOf(personas, this) != -1;
  }

  public static void MostrarEstadisticas()
  {
    Console.WriteLine($"Hay {NumPersonas} personas creadas");
  }

  public object Clone()
  {
    return new Persona(this);
  }

  public void Hablar()
  {
    Console.WriteLine($"Hola, me llamo {Nombre}");
  }

  public override string ToString()
  {
    return $"{Nombre}, {Edad} años";
  }
}
