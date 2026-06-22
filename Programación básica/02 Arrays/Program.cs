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