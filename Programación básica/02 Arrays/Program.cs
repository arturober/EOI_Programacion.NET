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


// Ejercicio para practicar: Imprimir un tablero de 3x3 de un juego de Tres en Raya usando un array 2D.
char[,] tresenraya = {
    // 0    1     2  
    { 'O', '-',  '-' }, // 0
    { 'O', 'X',  '-' }, // 1
    { 'O', '-',  'X' }  // 2
};


Console.WriteLine("Imprimimos las casilla del tablero una a una:");

// Fila 0
Console.Write(tresenraya[0, 0]); // Columna 0 // O
Console.Write(tresenraya[0, 1]); // Columna 1 // -
Console.Write(tresenraya[0, 2]); // Columna 2 // -

Console.WriteLine(); // Salto de línea

// Fila 1
Console.Write(tresenraya[1, 0]); // Columna 0 // O
Console.Write(tresenraya[1, 1]); // Columna 1 // X
Console.Write(tresenraya[1, 2]); // Columna 2 // -

Console.WriteLine(); // Salto de línea

// Fila 2
Console.Write(tresenraya[2, 0]); // Columna 0 // O
Console.Write(tresenraya[2, 1]); // Columna 1 // -
Console.Write(tresenraya[2, 2]); // Columna 2 // X

Console.WriteLine(); // Salto de línea


Console.WriteLine("Imprimimos todo el tablero con un bucle:");

for (int i = 0; i < 3; i++) // 3 filas
{
    for (int j = 0; j < 3; j++) // 3 columnas
    {
        Console.Write(tresenraya[i, j]); // Imprime cada casilla
    }
    Console.WriteLine();
}

// O - - 
// O X - 
// O - X

// Arrays con elementos de diferentes tipos
object[] arrayDeObjetos = { 1, "Hola", 3.14, true, 'A' };

Console.WriteLine(arrayDeObjetos[0]); // 1
Console.WriteLine(arrayDeObjetos[1]); // Hola
Console.WriteLine(arrayDeObjetos[2]); // 3.14
Console.WriteLine(arrayDeObjetos[3]); // True
Console.WriteLine(arrayDeObjetos[4]); // A