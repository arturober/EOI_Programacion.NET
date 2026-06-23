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
