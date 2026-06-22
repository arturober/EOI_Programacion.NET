// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");
Console.WriteLine("Me llamo Arturo");

int num = 23; // Variable
num = 13; // Reasignación de valor

const double PI = 3.1416; // Constante. No se puede cambiar el valor

Console.WriteLine(num); // 13

int num2 = num / 3;
Console.WriteLine(num2); // 4
Console.WriteLine(num % 3); // 1
Console.WriteLine(num / 3.0); // 4,33333333

string nombre = "Pepe";
Console.WriteLine(nombre); // Pepe
Console.WriteLine("Hola " + nombre); // Pepe

double precio = 2.32;
Console.WriteLine(precio + "€");

bool cierto = false;
Console.WriteLine("¿Es cierto?: " + cierto);

// double: el tipo decimal más común
double pi = 3.141592653589793;
double altura = 1.75;

// float: menos precisión, necesita la "f" al final
float velocidad = 120.5f;

// decimal: alta precisión para dinero (necesita la "m" al final)
decimal salario = 2500.50m;

// Sin interpolación
Console.WriteLine("Pi: " + pi);

// Con interpolación
Console.WriteLine($"Pi: {pi}");
Console.WriteLine($"Altura: {altura} metros");
Console.WriteLine($"Velocidad: {velocidad} km/h");
Console.WriteLine($"Precio: {precio}€");
Console.WriteLine($"Salario: {salario}€");
