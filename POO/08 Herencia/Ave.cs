class Ave(string nombre, double peso, bool puedeVolar) : Animal(nombre, peso)
{
  public bool PuedeVolar { get; init; } = puedeVolar;

  public override void Comer()
  {
    // Definimos que un ave aumenta su peso un 5% siempre al comer
    Peso *= 1.05;
    Console.WriteLine($"Pio pio. He comido y ahora peso {Peso} kilos");
  }
}
