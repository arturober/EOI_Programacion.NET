class Circulo(double radio): IFigura
{
  public double Radio { get; init => field = value < 0.1 ? 0.1 : value; } = radio;

  public double Perimetro => 2 * Math.PI * Radio;

  public double Area => Math.PI * Math.Pow(Radio, 2);

  public override string ToString()
  {
    return $"[Círculo -> Radio: {Radio} - perímetro: {Perimetro:F2} - área: {Area:F2}]";
  }
}
