class Mamifero(string nombre, double peso, bool carnivoro) : Animal(nombre, peso)
{
  public bool Carnivoro { get; init; } = carnivoro;

  public override void Comer()
  {
    base.Comer(); // Ejecuta el método comer original (Animal)
    Console.WriteLine($"He comido: {(Carnivoro ? "carne" : "hierba")}");
  }

    public override string TipoAnimal() => "Mamífero";
}
