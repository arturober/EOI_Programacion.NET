// Estructura de control if muy simple
Console.WriteLine("\nIf muy simple:");

int edad = 20;

if (edad >= 18)
{
    Console.WriteLine("Eres mayor de edad.");
    Console.WriteLine("Puedes votar.");
}

// Estructura de control if-else
Console.WriteLine("\nIf else muy simple:");

int edad2 = 15;

if (edad2 >= 18)
{
    Console.WriteLine("Eres mayor de edad.");
}
else
{
    Console.WriteLine("Eres menor de edad.");
}

// Bucle while muy simple
Console.WriteLine("\nBucle while muy simple:");

// Contar del 1 al 5
int contador = 1;

while (contador <= 5)
{
    Console.WriteLine($"Contador: {contador}");
    contador++;
}
Console.WriteLine("¡Fin del bucle!");


// Un switch muy simple
Console.WriteLine("\nSwitch muy simple:");

string opcion = "";

while (opcion != "4")
{
    Console.WriteLine("\n═══ MENÚ ═══");
    Console.WriteLine("1. Saludar");
    Console.WriteLine("2. Mostrar fecha");
    Console.WriteLine("3. Mostrar hora");
    Console.WriteLine("4. Salir");
    Console.Write("Elige una opción: ");
    opcion = Console.ReadLine()!;

    switch (opcion)
    {
        case "1":
            Console.WriteLine("¡Hola, bienvenido!");
            break;
        case "2":
            Console.WriteLine($"Hoy es: {DateTime.Now:dd/MM/yyyy}");
            break;
        case "3":
            Console.WriteLine($"Son las: {DateTime.Now:HH:mm:ss}");
            break;
        case "4":
            Console.WriteLine("¡Hasta luego!");
            break;
        default:
            Console.WriteLine("Opción no válida.");
            break;
    }
}

// Bucle for muy simple
Console.WriteLine("\nBucle for muy simple:");

// Sintaxis: for (inicialización; condición; actualización)
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Iteración {i}");
}

// Equivalente a un bucle while
Console.WriteLine("\nBucle while equivalente al for anterior:");

int j = 0;
while (j < 5)
{
    Console.WriteLine($"Iteración {j}");
    j++;
}


// Break y continue
Console.WriteLine("\nBreak y continue:");

Console.WriteLine("\nBreak:");
// Buscar el primer número divisible por 7 entre 1 y 100
for (int i = 1; i <= 100; i++)
{
    if (i % 7 == 0)
    {
        Console.WriteLine($"Primer múltiplo de 7: {i}");
        break;  // Sale del bucle
    }
}
// Salida: Primer múltiplo de 7: 7

Console.WriteLine("\nContinue:");
// Mostrar solo los números impares del 1 al 10
for (int i = 1; i <= 10; i++)
{
    if (i % 2 == 0)
    {
        continue;  // Salta los pares
    }
    Console.Write($"{i} ");
}
// Salida: 1 3 5 7 9

