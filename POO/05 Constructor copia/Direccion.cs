class Direccion
{
  public string Calle { get; set; }
  public string CP { get; set; }
  public int Numero { get; set; }

  public Direccion(string calle, string cp, int numero)
  {
    Calle = calle;
    CP = cp;
    Numero = numero;
  }

  public Direccion(Direccion d)
  {
    Calle = d.Calle;
    CP = d.CP;
    Numero = d.Numero;
  }
}
