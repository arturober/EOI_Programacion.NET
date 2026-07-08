class Persona: ICloneable
{
  public static int NumPersonas { get; private set; } = 0;
  public string Nombre { get; set; }
  public int Edad { get; set => field = value < 0 ? field : value; }


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
    NumPersonas++;
  }

  public static Persona PersonaGenerica() => new Persona("Anónimo", 20);

  public void Saluda() => Console.WriteLine($"{Nombre} tiene {Edad} años");

  public bool EstoyEnArray(Persona[] personas) => Array.IndexOf(personas, this) != -1;


  public static void MostrarEstadisticas() => Console.WriteLine($"Hay {NumPersonas} personas creadas");

  public object Clone() => new Persona(this);

  public void Hablar() => Console.WriteLine($"Hola, me llamo {Nombre}");

  public override string ToString() => $"{Nombre}, {Edad} años";
}
