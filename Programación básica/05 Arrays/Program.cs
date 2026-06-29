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


var c = Console.ReadKey(true).KeyChar;
Console.WriteLine(c);

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
  else {
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

if ( interruptores is [true, false, false, true] )
{
  Console.WriteLine("Los interruptores son correctos");
}

string texto = """
  Contenido del archivo
  con varias líneas.
  """;

var file = new System.IO.StreamReader("archivo.txt");
