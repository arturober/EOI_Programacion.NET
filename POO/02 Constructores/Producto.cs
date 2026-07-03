class Producto(string nombre, double precio)
{
  public string? Nombre
  {
    get => field?.ToUpper();
    set
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(value);
      field = value;
    }
  } = nombre;

  public double Precio { get; set => field = value < 0.0 ? 0.0 : value; } = precio;
}
