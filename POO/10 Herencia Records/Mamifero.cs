public record Mamifero(string Nombre, double Peso, bool Carnivoro) : Animal(Nombre, Peso)
{
  public override string TipoAnimal() => "Mamífero";
}
