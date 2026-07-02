class Producto
{
  public string Nombre
  {
    get => field.ToUpper();
    set
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(value);
      field = value;
    }
  }
  public double Precio { get; set => field = value < 0.0 ? 0.0 : value; }
}
