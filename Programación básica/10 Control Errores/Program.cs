try
{
  int a = 0;
  int b = 2 / a; // ERROR: va al catch directamente
  Console.WriteLine("No ha habido error"); // No se ejecuta
}
catch (DivideByZeroException e)
{
  Console.WriteLine($"Excepción de división por cero capturada: {e.Message}");
}

int i = 0;
int[] array = [12, 23, 4, 6];
try
{
  for (; i < 8; i++)
  {
    array[i] = array[i] / array[i + 1];
  }
}
catch (DivideByZeroException)
{
  Console.WriteLine($"Error: División por cero en posición: {i + 1}");
}
catch (IndexOutOfRangeException)
{
  Console.WriteLine($"Error: Índice fuera del array: {i + 1}");
}

void saluda(string nombre)
{
  // if(nombre == null || nombre == "")
  // {
  //   throw new ArgumentNullException(nameof(nombre), "El nombre es nulo o está vacío");
  // }
  ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
  Console.WriteLine($"Hola {nombre}");
}

try
{
  saluda("Juan");
  saluda("");
}
catch (ArgumentException e)
{
  Console.WriteLine($"Se produjo error en parámetro {e.ParamName} -> {e.Message}");
}

void Ejercicio8()
{
  Console.Write("Nombre de archivo a mostrar: ");
  string? archivo = Console.ReadLine();

  string contenido = File.ReadAllText(Path.Join("archivos", archivo));
  Console.WriteLine(contenido);
}

try
{
  Ejercicio8();
}
catch(FileNotFoundException e)
{
  Console.WriteLine($"Archivo no encontrado {e.FileName}");
}

Console.WriteLine("Final de programa");
