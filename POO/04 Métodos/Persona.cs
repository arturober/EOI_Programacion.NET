class Persona(string nombre, int edad)
{
  public string Nombre { get; set; } = nombre;
  public int Edad { get; set => field = value < 0 ? field : value; } = edad;

  public void Saluda()
  {
    Console.WriteLine($"{Nombre} tiene {Edad} años");
  }
}
