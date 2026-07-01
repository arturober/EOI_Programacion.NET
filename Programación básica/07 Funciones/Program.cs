
using ResultadoBuscar = (bool Encontrado, int Posicion);

void Saluda()
{
  Console.WriteLine("Hello, World!");
  Console.WriteLine("¿Cómo estás?");
}

// Función con valor por defecto para el parámetro (opcional)
void SaludaNombre(string nombre = "Anónimo")
{
  Console.WriteLine($"Hola {nombre}");
}

int Suma(int n1, int n2)
{
  return n1 + n2;
}

void InfoRectangulo(double alto, double ancho)
{
  Console.WriteLine($"Alto: {alto}, ancho: {ancho}. Área: {alto * ancho:F2}");
}

void Coordenadas3D(int x = 0, int y = 0, int z = 0)
{
  Console.WriteLine($"Coordenadas: {x}, {y}, {z}");
}

int SumaArray(int[] nums)
{
  Console.WriteLine($"He recibido {nums.Length} números");
  int total = 0;
  for (int i = 0; i < nums.Length; i++)
  {
    total += nums[i];
  }
  return total;
}

int SumaArray2(params int[] nums)
{
  Console.WriteLine($"He recibido {nums.Length} números");
  int total = 0;
  for (int i = 0; i < nums.Length; i++)
  {
    total += nums[i];
  }
  return total;
}

Saluda();
SaludaNombre("Pepe"); // Hola Pepe
SaludaNombre(); // Hola Anónimo -> Llamamos con valor por defecto
Console.WriteLine($"4 + 7 = {Suma(4, 7)}");

// Paso de parámetros por nombre
InfoRectangulo(6.5, 9.2);
InfoRectangulo(ancho: 9.2, alto: 6.5);

Coordenadas3D(); // Coordenadas: 0, 0, 0
Coordenadas3D(z: 5); // Coordenadas: 0, 0, 5

int total = SumaArray([5, 6, 2, 7, 12]);
Console.WriteLine($"Total array: {total}");

int total2 = SumaArray2(5, 6, 2, 7, 12);
Console.WriteLine($"Total array: {total2}");

Console.WriteLine("Programa terminado");

// Ejercicio 2 - Parte 1

void LongitudCadena(string cadena, int a, int b)
{
  if (cadena.Length >= a && cadena.Length <= b)
  {
    Console.WriteLine("Longitud cadena se encuentra entre los 2 numeros.");
  }
  else
  {
    Console.WriteLine("Longitud cadena no se encuentra entre los 2 numeros.");
  }
}

LongitudCadena("Cuerpoespialidoso", 10, 20);
LongitudCadena("Hola", 10, 17);

/*** Parámetros por referencia ***/
void CambiaNum(ref int n)
{
  n = 99;
}

int num = 1;
CambiaNum(ref num);
Console.WriteLine(num); // 99 (Cuidado con ref!)

/*** Parámetros no primitivos. Se pueden modificar a nivel interno ***/
void CambiaArray(int[] nums)
{
  nums[0] = 99;
}

int[] arrayNums = [1, 2, 3, 4];
CambiaArray(arrayNums);
Console.WriteLine(string.Join(", ", arrayNums)); // 99, 2, 3, 4 (Cuidado con las modificaciones internas)

/*** Parámetro de salida (out). Permite que la función devuelva más de un valor. No es recomendable usarlo ***/
bool BuscaValor(int[] arrayBuscar, int valor, out int posicion)
{
  bool encontrado = false;
  posicion = -1; // Por si no lo encuentra
  for (int i = 0; i < arrayBuscar.Length && !encontrado; i++)
  {
    if (arrayBuscar[i] == valor)
    {
      posicion = i;
      encontrado = true;
    }
  }
  return encontrado;
}

int[] array = [5, 12, 42, 15, 8];
int posicion;
bool encontrado = BuscaValor(array, 15, out posicion);
if (encontrado)
{
  System.Console.WriteLine($"Encontrado el valor 15 en la posición {posicion}");
}

/*** Ejemplo de devolución de 2 valores usando tuplas ***/
ResultadoBuscar BuscaValor2(int[] arrayBuscar, int valor)
{
  bool encontrado = false;
  posicion = -1; // Por si no lo encuentra
  for (int i = 0; i < arrayBuscar.Length && !encontrado; i++)
  {
    if (arrayBuscar[i] == valor)
    {
      posicion = i;
      encontrado = true;
    }
  }
  return (encontrado, posicion);
}

ResultadoBuscar res = BuscaValor2(array, 15);
if (res.Encontrado)
{
  Console.WriteLine($"Encontrado el valor 15 en la posición {res.Posicion}");
}
