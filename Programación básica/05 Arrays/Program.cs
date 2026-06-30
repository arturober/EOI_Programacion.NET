using System.Numerics.Tensors;

int[] numeros = new int[5];
numeros[0] = 15;
numeros[1] = 3;
numeros[2] = 8;

Console.WriteLine(String.Join(',', numeros)); // 15,3,8,0,0

string[] cadenas = new string[5];
cadenas[0] = "perro";
cadenas[1] = "gato";
cadenas[2] = "loro";

Console.WriteLine(String.Join(',', cadenas)); // perro,gato,loro,,

// Sintaxis simplificada para crear colecciones (expresión de colección)
int[] numeros2 = [12, 23, 53, 5, 92];
List<int> listaNum = [4, 23, 6, 18, 9, 13];

Console.WriteLine("-- Recorremos con for --");
for (int i = 0; i < numeros2.Length; i++)
{
  Console.WriteLine(numeros2[i]);
}

Console.WriteLine("-- Recorremos con foreach --");
foreach (var num in numeros2)
{
  Console.WriteLine(num);
}

Console.WriteLine("-- Recorremos con Array.ForEach --");
Array.ForEach(numeros2, n => Console.WriteLine(n));

int sum = 0;

foreach (int n in numeros2)
{
  sum += n;
}

Console.WriteLine($"Suma de numeros: {sum}");

// Buscar valor en array
string[] nombres = ["Juan", "Ana", "Pedro", "Eva", "Paco"];
bool encontrado = false; // Nos indicará si hemos encontrado el valor
string buscar = "Pedro"; // Valor a buscar en el array

// La condición también incluye que no hayamos encontrado lo que buscamos
for (int i = 0; i < nombres.Length && !encontrado; i++)
{
  if (nombres[i] == buscar)
  {
    encontrado = true;
  }
}

if (encontrado)
{
  Console.WriteLine($"El nombre {buscar} está en el array");
}
else
{
  Console.WriteLine($"{buscar} no encontrado...");
}

// Búsqueda con Exists
int[] edades = [23, 23, 16, 93, 39, 12];
bool existeMenor = Array.Exists(edades, n => n < 18); // True

// Ordenar arrays
Array.Sort(numeros2);
Console.WriteLine(string.Join(", ", numeros2)); // 5, 12, 23, 53, 92

Array.Sort(nombres);
Console.WriteLine(string.Join(", ", nombres));

string[] palabras = ["pelota", "aro", "zorro", "casamiento", "melonar", "arboleda"];
// Ordenar alfabéticamente a la inversa
Array.Sort(palabras);
Array.Reverse(palabras);
Console.WriteLine(string.Join(", ", palabras));

// Ordenar por longitud de palabra
Array.Sort(palabras, (p1, p2) => p1.Length - p2.Length);
Console.WriteLine(string.Join(", ", palabras));

/* EJERCICIO 1 (Parte 2) */
Console.WriteLine(" --- EJERCICIO 1 (Parte 2) --- ");

string[] nombresConsola = new string[10];

int contador = 0;

while (contador < nombresConsola.Length)
{
  Console.Write($"Nombre {contador + 1}: ");
  var nombre = Console.ReadLine();

  if (string.IsNullOrWhiteSpace(nombre))
  {
    Console.WriteLine("El nombre está vacío");
  }
  else if (Array.Exists(nombresConsola, n => n == nombre))
  {
    Console.WriteLine("El nombre ya está repetido");
  }
  else
  {
    nombresConsola[contador++] = nombre;
  }
}

// RANGOS
Console.WriteLine(" --- RANGOS --- ");
char[] letras = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
char[] letras2 = letras[3..6]; // Copiamos posiciones de la 3 (incluida) a la 6 (no incluida)

Console.WriteLine(string.Join(',', letras2));
Console.WriteLine(string.Join(',', letras[3..])); // Desde la 3 (incluida)
Console.WriteLine(string.Join(',', letras[..3])); // Has la 3 (no incluida)
Console.WriteLine(string.Join(',', letras[^3..])); // Las 3 últimas posiciones

// Patrones de lista
Console.WriteLine(" --- Patrones de lista --- ");
bool[] interruptores = [true, false, false, true];

if (interruptores is [true, false, false, true])
{
  Console.WriteLine("Los interruptores son correctos");
}

/* Operador Spread */
Console.WriteLine(" --- Operador Spread --- ");

string[] frutas = ["Piña", "Melón", "Plátano"];
string[] frutas2 = ["Manzana", "Pera", "Sandía"];

// Array que contiene los elementos de frutas + frutas2 + otra fruta
string[] frutas3 = [.. frutas, "Ciruela", .. frutas2];
Console.WriteLine(string.Join(", ", frutas3));

// Otra forma más clásica de hacerlo
Console.WriteLine(string.Join(", ", frutas.Append("Ciruela").Concat(frutas2)));

/** --- Ejercicio 7 - Parte 1 --- **/
Console.WriteLine("--- Ejercicio 7 - Parte 2 ---");

int[] nums1 = [2, 54, 15, 6, 37, 87, 24];
int[] nums2 = [7, 23, 15, 87, 23, 38, 7];

int[] nums3 = new int[nums1.Length];

for(int i = 0; i < nums1.Length; i++)
{
  nums3[i] = nums1[i] + nums2[i];
}

Console.WriteLine(string.Join(", ", nums3));

// Alternativa con Linq (todavía no lo hemos visto)
int[] nums4 = nums1.Zip(nums2, (n1, n2) => n1 + n2).ToArray();
Console.WriteLine(string.Join(", ", nums4));

// Alternativa con TensorPrimitives (operaciones en paralelo)
int[] nums5 = new int[nums1.Length];
TensorPrimitives.Add(nums1, nums2, nums5);
Console.WriteLine(string.Join(", ", nums5));

/*** Ejercicio 3 - Parte 1 ***/

int suma = 0;
foreach (var n in nums1)
{
  suma += n;
}

double media = suma / (double) nums1.Length;

Console.WriteLine($"Media {media:F2}");
Console.Write("Números mayores: ");
foreach (var n in nums1)
{
  if(n > media)
  {
    Console.Write(n + " ");
  }
}
Console.WriteLine();

// Forma moderna/avanzada
// double media = nums1.Average();
// Console.WriteLine($"Media {media:F2}");
// Console.WriteLine($"Números mayores: {string.Join(", ", nums1.Where(n => n > media))}");
