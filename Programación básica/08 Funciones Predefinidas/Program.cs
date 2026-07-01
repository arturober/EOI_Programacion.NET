// Funciones matemáticas
Console.WriteLine(" --- Funciones matemáticas --- ");

int max = int.MaxValue;
// Multiplicación de números muy grandes (no caben en int y devuelve long)
long res = Math.BigMul(max, 100);
Console.WriteLine(res);

Console.WriteLine(Math.Pow(4, 3)); // 64
Console.WriteLine(Math.Round(10 / 3.0, 2)); // 3,33
Console.WriteLine(Math.Max(6, 3));

int[] nums = [2, 4, 12, 24, 6, 34, 9];
// Buscar el máximo
int maximo = 0;
foreach (int num in nums)
{
  maximo = Math.Max(maximo, num);
}
Console.WriteLine(maximo);

int maximo2 = nums.Aggregate((max, n) => Math.Max(max, n));
Console.WriteLine(maximo2);

Console.WriteLine(nums.Max());

// Funciones de strings
Console.WriteLine(" --- Funciones strings --- ");

string s1 = "abc";
string s2 = "def";
string s3 = "ghi";
string concatenada = string.Concat(s1, s2, s3);
Console.WriteLine(concatenada); // abcdefghi

string[] palabras = ["perro", "gato", "pájaro", "avestruz", "koala", "cocodrilo"];
Console.WriteLine(string.Concat(palabras));
Console.WriteLine(string.Join(" - ", palabras));

string[] ordenados = [.. palabras]; // Clonamos el array original
Array.Sort(ordenados, (p1, p2) => string.Compare(p2, p1)); // Ordenamos al revés (descendente)
Console.WriteLine(string.Join(", ", ordenados));

string s = "Mi perro se llama Comeniños";
Console.WriteLine(s.IndexOf("perro")); // 3
Console.WriteLine(s.IndexOf("gato")); // -1

// Función que devuelve cuantas veces aparece una subcadena en una cadena
int VecesAparece(string cadena, string buscar)
{
  int pos = 0;
  int coincidencias = 0;
  while (pos != -1)
  {
    pos = cadena.IndexOf(buscar, pos);
    if (pos != -1)
    {
      coincidencias++;
      pos++; // Buscamos a partir de la siguiente posición
    }
  }
  return coincidencias;
}

int veces = VecesAparece("Mi casa es roja y mi coche es verde", "a");
Console.WriteLine($"{veces} veces");

Console.WriteLine(s.Remove(3, 5));
Console.WriteLine(s.Insert(3, "super "));
Console.WriteLine(s.Replace("perro", "koala"));

Console.WriteLine(s.Substring(3, 5));
Console.WriteLine(s[3..8]);

Console.WriteLine(s.ToUpper());

foreach (var animal in palabras)
{
  Console.WriteLine($"{animal.PadLeft(14)} -> Longitud: {animal.Length}");
}

foreach (var animal in palabras)
{
  Console.WriteLine($"{animal,14} -> Longitud: {animal.Length}");
}

string items = "Manzanas;Peras;Zanahoria;Tomates";
string[] itemsArray = items.Split(";");

Console.WriteLine(itemsArray[2]); // Zanahoria

// Operaciones con arrays
Console.WriteLine(" --- Funciones arrays --- ");

int[] unos = new int[10];
Array.Fill(unos, 1);
Console.WriteLine(String.Join(", ", unos)); // 1, 1, 1, 1, 1, 1, 1, 1, 1, 1

int[] numeros = { 2, 4, 8, 12, 24 };
Console.WriteLine(Array.IndexOf(numeros, 8)); // 2

Array.Reverse(numeros);
Console.WriteLine(string.Join(", ", numeros)); // 24, 12, 8, 4, 2

Array.Sort(numeros);
Console.WriteLine(string.Join(", ", numeros)); // 2, 4, 8, 12, 24

int[] nums2 = [5, 14, 54, 19, 34, 53, 27];
Console.WriteLine(Array.Find(nums2, n => n % 3 == 0)); // 54
int[] divisibles3 = Array.FindAll(nums2, n => n % 3 == 0);
Console.WriteLine(string.Join(", ", divisibles3)); // 54, 27

// Funciones de fecha
Console.WriteLine(" --- Funciones de fecha --- ");

var fecha = new DateTime(2019, 8, 12, 12, 30, 0);
var fecha2 = DateTime.Parse("2016-03-02T16:04:00");
Console.WriteLine(fecha.ToString()); // 12/8/19 12:30:00
Console.WriteLine(fecha2.ToString()); // 2/3/2016 16:04:00

DateTime fecha3 = DateTime.Parse("31/12/2018");
Console.WriteLine(fecha3.ToString()); // 31/12/18 0:00:00

DateOnly fecha4 = DateOnly.ParseExact("10/25/2018", "mm/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
Console.WriteLine(fecha4.ToString()); // 25/1/2018

DateTime ayer = DateTime.Now.AddDays(-1);
string[] dias = ["Domingo", "Lunes", "Martes", "Miércoles","Jueves", "Viernes", "Sábado"];
string diaAyer = dias[(int)ayer.DayOfWeek];

Console.WriteLine($"Ayer fue {diaAyer}, {ayer.Day:D2}/{ayer.Month:D2}/{ayer.Year}");



