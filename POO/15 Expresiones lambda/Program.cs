void HacerOperacion(double n1, double n2, OperaNums operacion)
{
  Console.WriteLine(operacion(n1, n2));
}

OperaNums sumar = (n1, n2) => n1 + n2;
OperaNums restar = (n1, n2) => n1 - n2;
OperaNums multiplicar = (n1, n2) => n1 * n2;
OperaNums dividir = (n1, n2) => n1 / n2;

Console.WriteLine(sumar(3.5, 6.25));

HacerOperacion(24.4, 6.34, restar);
HacerOperacion(24.4, 6.34, multiplicar);
HacerOperacion(24.4, 6.34, dividir);

//------------------
Func<int, int, int> sumarFunc = (n1, n2) => n1 + n2;
var sumarFunc2 = (int n1, int n2) => n1 + n2; // Hace lo mismo

Predicate<int> mayor18 = (edad) => edad >= 18;

Action<string> imprimeString = (dato) => Console.WriteLine(dato);

Console.WriteLine(sumarFunc(4, 7));
Console.WriteLine(mayor18(54));

List<int> edades = [23, 15, 65, 28, 23, 16];
edades.RemoveAll(mayor18);
Console.WriteLine(string.Join(", ", edades)); // 15, 16

// La función recibe una lista de enteros y un número
Action<List<int>, int> numDivisibles = (lista, divisor) =>
{
  int veces = lista.FindAll(n => n % divisor == 0).Count;
  Console.WriteLine($"Hay {veces} números divisibles entre {divisor}");
};

List<int> numeros = [16, 36, 15, 86, 72, 104, 205, 115];
numDivisibles(numeros, 3); // Hay 3 números divisibles entre 3

// Predicado

Predicate<Persona> mayorEdad = p => p.Edad >= 18;
var pepe = new Persona("Pepito", 23);
Console.WriteLine(mayorEdad(pepe)); // True

// Función

Func<int, int, int> multiplica = (n1, n2) => n1 * n2;
Func<string, string, int> difLongitud = (s1, s2) => Math.Abs(s1.Length - s2.Length);
Func<Cuadrado, Cuadrado, Cuadrado> sumaCuadrados = (c1, c2) => new Cuadrado(c1.Lado + c2.Lado);

Console.WriteLine(multiplica(2, 5)); // 10
Console.WriteLine(difLongitud("caracola", "cebra")); // 3

var cu1 = new Cuadrado(4);
var cu2 = new Cuadrado(6);
var sumaCu = sumaCuadrados(cu1, cu2);
Console.WriteLine($"Lado: {sumaCu.Lado}, área: {sumaCu.Area}");

// El tipo delegado define una función
delegate double OperaNums(double n1, double n2);


