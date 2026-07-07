public record Mamifero(string Nombre, double Peso, bool Carnivoro) : Animal(Nombre, Peso)
{
  public override string TipoAnimal() => "Mamífero";

  public override void Hablar()
  {
    Console.WriteLine($"{Nombre} hace un extraño sonido");
  }
}
