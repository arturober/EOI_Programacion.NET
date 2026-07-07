abstract class Animal(string nombre, double peso): IHablador
{
  public double Peso { get; set; } = peso;
  public string Nombre { get; init; } = nombre;

  public virtual void Comer()
  {
    Peso += new Random().NextDouble() * 0.5;
    Console.WriteLine($"Ñam ñam. Ahora peso {Peso:F2} kilos");
  }

  public override string ToString() => $"{Nombre}: {Peso:F2} kilos";

  public override int GetHashCode() => Tuple.Create(Nombre, Peso).GetHashCode();

  public override bool Equals(object? obj)
  {
    // Comprobamos que el objeto no es nulo y sea Animal o derivado
    if (obj == null || obj is not Animal) return false;
    var a = (Animal)obj; // Casting al tipo Animal
    return Nombre == a.Nombre && Peso == a.Peso; // mismo nombre y peso
  }

  public static bool operator ==(Animal a1, Animal a2) => a1.Equals(a2);

  public static bool operator !=(Animal a1, Animal a2) => !a1.Equals(a2);

  public Animal Clone() => (Animal)MemberwiseClone();

  public abstract string TipoAnimal();

  public abstract void Hablar();
}
