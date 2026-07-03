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
}
