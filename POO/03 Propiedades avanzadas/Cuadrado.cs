class Cuadrado
{
  public required double Lado { get; init => field = value < 0.1 ? 0.1 : value; }

  public double Area { get => Lado * Lado; }
}
