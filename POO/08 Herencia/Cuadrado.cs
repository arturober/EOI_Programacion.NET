class Cuadrado(double lado) : Figura
{
  public double Lado { get; init => field = value < 0.1 ? 0.1 : value; } = lado;

  public override double Perimetro => Lado * 4;

  public override double Area => Math.Pow(Lado, 2);

  public override string ToString()
  {
    return $"Cuadrado -> Lado: {Lado}, perímetro: {Perimetro:F2}, área: {Area:F2}";
  }
}
