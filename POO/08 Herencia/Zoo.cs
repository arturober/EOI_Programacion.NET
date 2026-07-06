class Zoo(string nombre, int capacidad)
{
  private string Nombre { get; } = nombre;
  private Animal[] animales = new Animal[capacidad];
  public int NumAnimales { get; private set; } = 0;

  public void AddAnimal(Animal a)
  {
    if (NumAnimales < animales.Length)
    {
      animales[NumAnimales++] = a;
    }
    else
    {
      Console.WriteLine("Error: El zoo ha alcanzado su capacidad máxima.");
    }
  }

  public Animal GetAnimal(int pos)
  {
    if (pos >= 0 && pos < animales.Length)
    {
      return animales[pos];
    }
    throw new IndexOutOfRangeException("El animal al que intentas acceder no existe");
  }

  public Zoo Clone()
  {
    var copiaZoo = (Zoo)MemberwiseClone();
    // También debemos generar un nuevo array (por defecto es el mismo)
    copiaZoo.animales = new Animal[animales.Length];
    for (int i = 0; i < NumAnimales; i++)
    {
      copiaZoo.animales[i] = animales[i].Clone();
    }
    return copiaZoo;
  }
}
