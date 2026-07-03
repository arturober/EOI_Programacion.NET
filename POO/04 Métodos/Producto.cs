class Producto
{
  public required string Nombre
  {
    get => field.ToUpper();
    set
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(value);
      field = value;
    }
  }
  public required double Precio { get; set => field = value < 0.0 ? 0.0 : value; }

  public double GetPrecioImpuesto()
  {
    return Precio * 1.21;
  }

  public double GetPrecioImpuesto(double impuesto)
  {
    return Precio * ( 1 + impuesto );
  }
}
