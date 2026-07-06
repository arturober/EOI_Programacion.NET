class Empleado(int rangoInicial)
{
  // atributo estático de solo lectura (valor compartido por todos los objetos)
  private static readonly string[] nombresRango = ["Junior", "Estándar", "Senior", "Experto", "Pro"];
  private int Rango { get; set => field = Math.Clamp(value, 1, 5); } = rangoInicial;
  public string NombreRango { get => nombresRango[Rango - 1]; }

  public void Ascender()
  {
    Rango++;
    Console.WriteLine($"Empleado ascendido al rango {Rango}");
  }
}
