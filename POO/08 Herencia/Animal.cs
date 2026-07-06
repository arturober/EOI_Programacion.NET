class Animal(string nombre, double peso)
{
  public double Peso { get; protected set; } = peso;
  public string Nombre { get; init; } = nombre;

  public virtual void Comer()
  {
    Peso += new Random().NextDouble() * 0.5;
    System.Console.WriteLine($"Ñam ñam. Ahora peso {Peso:F2} kilos");
  }
}
