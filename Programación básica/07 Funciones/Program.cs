
using ResultadoBuscar = (bool Encontrado, int Posicion);

void Saluda()
{
  Console.WriteLine("Hello, World!");
  Console.WriteLine("¿Cómo estás?");
}

// Función con valor por defecto para el parámetro (opcional)
void SaludaNombre(string nombre = "Anónimo")
{
  Console.WriteLine($"Hola {nombre}");
}

int Suma(int n1, int n2)
{
  return n1 + n2;
}

void InfoRectangulo(double alto, double ancho)
{
  Console.WriteLine($"Alto: {alto}, ancho: {ancho}. Área: {alto * ancho:F2}");
}

void Coordenadas3D(int x = 0, int y = 0, int z = 0)
{
  Console.WriteLine($"Coordenadas: {x}, {y}, {z}");
}

int SumaArray(int[] nums)
{
  Console.WriteLine($"He recibido {nums.Length} números");
  int total = 0;
  for (int i = 0; i < nums.Length; i++)
  {
    total += nums[i];
  }
  return total;
}

int SumaArray2(params int[] nums)
{
  Console.WriteLine($"He recibido {nums.Length} números");
  int total = 0;
  for (int i = 0; i < nums.Length; i++)
  {
    total += nums[i];
  }
  return total;
}

Saluda();
SaludaNombre("Pepe"); // Hola Pepe
SaludaNombre(); // Hola Anónimo -> Llamamos con valor por defecto
Console.WriteLine($"4 + 7 = {Suma(4, 7)}");

// Paso de parámetros por nombre
InfoRectangulo(6.5, 9.2);
InfoRectangulo(ancho: 9.2, alto: 6.5);

Coordenadas3D(); // Coordenadas: 0, 0, 0
Coordenadas3D(z: 5); // Coordenadas: 0, 0, 5

int total = SumaArray([5, 6, 2, 7, 12]);
Console.WriteLine($"Total array: {total}");

int total2 = SumaArray2(5, 6, 2, 7, 12);
Console.WriteLine($"Total array: {total2}");

Console.WriteLine("Programa terminado");

// Ejercicio 2 - Parte 1

void LongitudCadena(string cadena, int a, int b)
{
  if (cadena.Length >= a && cadena.Length <= b)
  {
    Console.WriteLine("Longitud cadena se encuentra entre los 2 numeros.");
  }
  else
  {
    Console.WriteLine("Longitud cadena no se encuentra entre los 2 numeros.");
  }
}

LongitudCadena("Cuerpoespialidoso", 10, 20);
LongitudCadena("Hola", 10, 17);

/*** Parámetros por referencia ***/
void CambiaNum(ref int n)
{
  n = 99;
}

int num = 1;
CambiaNum(ref num);
Console.WriteLine(num); // 99 (Cuidado con ref!)

/*** Parámetros no primitivos. Se pueden modificar a nivel interno ***/
void CambiaArray(int[] nums)
{
  nums[0] = 99;
}

int[] arrayNums = [1, 2, 3, 4];
CambiaArray(arrayNums);
Console.WriteLine(string.Join(", ", arrayNums)); // 99, 2, 3, 4 (Cuidado con las modificaciones internas)

/*** Parámetro de salida (out). Permite que la función devuelva más de un valor. No es recomendable usarlo ***/
bool BuscaValor(int[] arrayBuscar, int valor, out int posicion)
{
  bool encontrado = false;
  posicion = -1; // Por si no lo encuentra
  for (int i = 0; i < arrayBuscar.Length && !encontrado; i++)
  {
    if (arrayBuscar[i] == valor)
    {
      posicion = i;
      encontrado = true;
    }
  }
  return encontrado;
}

int[] array = [5, 12, 42, 15, 8];
int posicion;
bool encontrado = BuscaValor(array, 15, out posicion);
if (encontrado)
{
  System.Console.WriteLine($"Encontrado el valor 15 en la posición {posicion}");
}

/*** Ejemplo de devolución de 2 valores usando tuplas ***/
ResultadoBuscar BuscaValor2(int[] arrayBuscar, int valor)
{
  bool encontrado = false;
  posicion = -1; // Por si no lo encuentra
  for (int i = 0; i < arrayBuscar.Length && !encontrado; i++)
  {
    if (arrayBuscar[i] == valor)
    {
      posicion = i;
      encontrado = true;
    }
  }
  return (encontrado, posicion);
}

ResultadoBuscar res = BuscaValor2(array, 15);
if (res.Encontrado)
{
  Console.WriteLine($"Encontrado el valor 15 en la posición {res.Posicion}");
}

/***** Funciones flecha / lambda *****/
// Función local / flecha: Solo pueden tener una única instrucción (return implícito)
int SumarFlecha(int n1, int n2) => n1 + n2;

Console.WriteLine($"3 + 4 =  {SumarFlecha(3, 4)}");

// Función o expresión lambda: Se apoyan en tipos "delegados" y se pueden pasar por parámetro (son variables)
Func<int, int, int> multiplicaLambda = (n1, n2) => n1 * n2;
Action saluda = () => Console.WriteLine("Hola!");
Predicate<int> esMenorEdad = (edad) => edad < 18;

Console.WriteLine($"3 * 4 =  {multiplicaLambda(3, 4)}");
saluda();
Console.WriteLine($"16 es menor de edad: {esMenorEdad(16)}");

string[] nombres = ["Paco", "Pepe", "Ana", "María", "Sara"];
// Posición del primer nombre que empieza por A
int primeroA = Array.FindIndex(nombres, n => n.StartsWith('A'));
if (primeroA != -1)
{
  Console.WriteLine($"Nombre con 'A' encontrado en {primeroA} -> {nombres[primeroA]}");
}

string[] nombresP = nombres.Where(n => n.StartsWith('P')).ToArray();

/**** Ejemplo Recursividad
La recursividad es una técnica de programación en la que una función se llama a sí misma
para resolver un problema dividiéndolo en subtareas más pequeñas y similares. Funciona de
manera análoga a las muñecas rusas (matrioskas): cada llamada abre una versión más pequeña del problema,
y el proceso continúa de forma ordenada hasta alcanzar un caso base, que es la condición de parada
obligatoria que detiene las llamadas y permite que la función empiece a resolver y devolver los resultados
finales sin caer en un bucle infinito.
****/

int FactorialRecursivo(int num)
{
  if(num == 1) return 1;

  return num * FactorialRecursivo(--num);
}

int factorial5 = FactorialRecursivo(5);
Console.WriteLine(factorial5);

int BusquedaOrdenada(int[] array, int numero, int posIni, int posFin)
{
  int medio = (posFin - posIni) / 2 + posIni;
  if(posIni > posFin) return -1;
  else if(array[medio] == numero) return medio; // Encontrado (devolvemos posición)
  else if(posFin == posIni) return -1; // No encontrado (el array ya no se puede dividir más)
  else if(array[medio] < numero)
  { // El número está más arriba
    return BusquedaOrdenada(array, numero, medio + 1, posFin);
  }
  else
  { // El número está más abajo
    return BusquedaOrdenada(array, numero, posIni, medio - 1);
  }
}

int[] numeros = [1, 5, 12, 18, 23, 32, 34, 46, 76, 89];
int posNum = BusquedaOrdenada(numeros, 46, 0, numeros.Length - 1);
Console.WriteLine($"Posición encontrado: {posNum}");
