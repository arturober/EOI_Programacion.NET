int a = 15;
int b = 4;

Console.WriteLine($"Suma:           {a} + {b} = {a + b}");      // 19
Console.WriteLine($"Resta:          {a} - {b} = {a - b}");      // 11
Console.WriteLine($"Multiplicación: {a} * {b} = {a * b}");      // 60
Console.WriteLine($"División:       {a} / {b} = {a / b}");      // 3  ← ¡Sin decimales!
Console.WriteLine($"Módulo (resto): {a} % {b} = {a % b}");      // 3

// ¿Es un número par o impar?
int numero = 7;
int resto = numero % 2;
Console.WriteLine($"{numero} % 2 = {resto}");  // 1 (impar, porque al dividir 7/2 sobra 1)

// Más ejemplos
Console.WriteLine($"10 % 3 = {10 % 3}");  // 1 (10 = 3×3 + 1)
Console.WriteLine($"20 % 5 = {20 % 5}");  // 0 (división exacta)
Console.WriteLine($"7 % 4 = {7 % 4}");    // 3 (7 = 4×1 + 3)


// Incremento y decremento
int contador = 10;

// contador = contador + 1
contador++;  // Ahora vale 11
Console.WriteLine(contador);  // 11

// contador = contador - 1
contador--;  // Ahora vale 10
Console.WriteLine(contador);  // 10

// Pre-incremento vs Post-incremento

Console.WriteLine("\nPre-incremento vs Post-incremento:");

int x = 5;
Console.WriteLine(x++);  // Muestra 5, DESPUÉS incrementa a 6
Console.WriteLine(x);    // 6

int y = 5;
Console.WriteLine(++y);  // Incrementa PRIMERO a 6, muestra 6
Console.WriteLine(y);    // 6

Console.WriteLine("\nOperadores de asignación compuesta:");

// Operadores de asignación compuesta
int puntos = 100;

// puntos = puntos + 50; 
puntos += 50;    // puntos = 100 + 50 = 150
Console.WriteLine($"Después de sumar 50: {puntos}");

// puntos = puntos - 30;
puntos -= 30;    // puntos = 150 - 30 = 120
Console.WriteLine($"Después de restar 30: {puntos}");

// puntos = puntos * 2;
puntos *= 2;     // puntos = 120 * 2 = 240
Console.WriteLine($"Después de multiplicar por 2: {puntos}");

// puntos = puntos / 3;
puntos /= 3;     // puntos = 240 / 3 = 80
Console.WriteLine($"Después de dividir entre 3: {puntos}");

// puntos = puntos % 7;
puntos %= 7;     // puntos = 80 % 7 = 3 (80 = 7×11 + 3)
Console.WriteLine($"Resto de dividir entre 7: {puntos}");


// Operadores de comparación
Console.WriteLine("\nOperadores de comparación:");

int edad = 20;
int required = 18;

Console.WriteLine($"edad == 20: {edad == 20}");     // True
Console.WriteLine($"edad != 18: {edad != 18}");     // True
Console.WriteLine($"edad > 18:  {edad > required}");// True
Console.WriteLine($"edad < 18:  {edad < required}");// False
Console.WriteLine($"edad >= 18: {edad >= required}");// True
Console.WriteLine($"edad <= 18: {edad <= required}");// False

// Comparación de strings
string nombre = "Ana";
Console.WriteLine($"nombre == \"Ana\": {nombre == "Ana"}");  // True
Console.WriteLine($"nombre != \"Luis\": {nombre != "Luis"}");// True
