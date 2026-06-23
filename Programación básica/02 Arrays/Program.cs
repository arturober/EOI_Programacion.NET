// Declarar un array de 5 enteros (todos valen 0 inicialmente)
int[] numeros = new int[5];

// Asignar valores individual por posición
numeros[0] = 18;
numeros[1] = 20;
numeros[2] = 22;
numeros[3] = 45;
numeros[4] = 17;

// Mostrar valores del array
// Posición: [ 0,  1,  2,  3,  4]
// Valor:    [18, 20, 22, 45, 17]
Console.WriteLine(numeros[0]);  // 18
Console.WriteLine(numeros[2]);  // 22
Console.WriteLine(numeros[4]);  // 17   

// Forma larga
int[] numeros2 = new int[] { 18, 20, 22, 45, 17 };

// Forma abreviada (recomendada)
int[] numeros3 = { 10, 20, 30, 40, 50 };

// Forma moderna con new[] (el compilador infiere el tipo)
var numeros4 = new[] { 10, 20, 30, 40, 50 };

// Arrays de otros tipos
string[] nombres = { "Ana", "Luis", "María", "Pedro" };
double[] precios = { 9.99, 15.50, 3.25, 7.80 };
bool[] respuestas = { true, false, true, true };

Console.WriteLine(numeros.Length);  // 5
Console.WriteLine(nombres.Length);  // 4
Console.WriteLine(precios.Length);  // 4
Console.WriteLine(respuestas.Length);  // 4
Console.WriteLine(nombres[2]);  // María
Console.WriteLine(precios[1]);  // 15.5

// Declarar una matriz 3x3
int[,] matriz = new int[3, 3];

// Asignar valores
matriz[0, 0] = 1; matriz[0, 1] = 2; matriz[0, 2] = 3;
matriz[1, 0] = 4; matriz[1, 1] = 5; matriz[1, 2] = 6;
matriz[2, 0] = 7; matriz[2, 1] = 8; matriz[2, 2] = 9;

// Forma directa
int[,] matriz2 = {
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
};

// Recorrer con bucles anidados
int filas = matriz2.GetLength(0);     // 3 filas
int columnas = matriz2.GetLength(1);  // 3 columnas

Console.WriteLine("Filas: " + filas);
Console.WriteLine("Columnas: " + columnas);
for (int i = 0; i < filas; i++)
{
    for (int j = 0; j < columnas; j++)
    {
        Console.Write($"{matriz2[i, j],4}");
    }
    Console.WriteLine();
}

// Ejercicio para practicar: Crear un array de 7 elementos que contenga los días de la semana. 
// Pedir al usuario que introduzca un número del 0 al 6 y mostrar el día correspondiente. 
// Si el número no es válido, mostrar un mensaje de error.
Console.Write("Introduce el día de la semana (0-6): ");
int dia = int.Parse(Console.ReadLine()!);

string[] diasSemana = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

while (dia < 0 || dia > 6)
{
    Console.Write("Número inválido. Introduce un número entre 0 y 6: ");
    dia = int.Parse(Console.ReadLine()!);
}
Console.WriteLine($"El día de la semana es: {diasSemana[dia]}");