int[,] matriz = {{2,3,4}, {2,1,6}, {3,6,1}, {2,6,8}};

for(int i = 0; i < matriz.GetLength(0); i++) // Recorre filas
{
  for(int j = 0; j < matriz.GetLength(1); j++) // Recorre columnas
  {
    Console.Write(matriz[i,j] + " ");
  }
  Console.WriteLine();
}

int[][] matriz2 = [[2,3,4], [2,1], [3,6,1], [2,6,8,9]];

for(int i = 0; i < matriz2.Length; i++) // Recorre filas
{
  for(int j = 0; j < matriz2[i].Length; j++) // Recorre columnas
  {
    Console.Write(matriz2[i][j] + " ");
  }
  Console.WriteLine();
}

/*** Ejercicio 4 - Parte 2 ***/
Console.WriteLine("Ejercicio 4 - Parte 2");

string[] nombres = ["Juan", "Pepe", "Marta"];
double[][] notas = [[2, 5.4, 7.6, 8], [3.5, 6.8, 9.4], [7.6, 9, 5.5, 6, 7.4]];

for(int i = 0; i < nombres.Length; i++)
{
  double media = 0;
  for(int j = 0; j < notas[i].Length; j++)
  {
    media += notas[i][j];
  }
  media /= notas[i].Length;

  Console.WriteLine($"{nombres[i]} - Media: {media:F2}");
}

/*** Recorrer string ***/
Console.WriteLine(" --- Recorrer string --- ");

string frase = "Hola qué tal";
Console.WriteLine(frase[3]);
Console.WriteLine(frase[3..8]);

for(int i = 0; i < frase.Length; i++)
{
  Console.WriteLine(frase[i]);
}
