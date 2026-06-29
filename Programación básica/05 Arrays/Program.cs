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
for(int i = 0; i < numeros2.Length; i++)
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


