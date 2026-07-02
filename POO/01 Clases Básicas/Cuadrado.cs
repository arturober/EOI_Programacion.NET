class Cuadrado
{
  public double Lado {get; set => field = value < 0.1 ? 0.1 : value; }

  public double Area { get => Lado * Lado; }
}
