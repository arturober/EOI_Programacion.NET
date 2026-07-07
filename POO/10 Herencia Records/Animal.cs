public abstract record Animal(string Nombre, double Peso): IHablador
{
  public abstract string TipoAnimal();

  public abstract void Hablar();
}
