List<int> nums = [3, 5, 6, 12, 7, 3, 8, 9, 23, 54];
List<int> pares = (from n in nums where n % 2 == 0 select n).ToList();
List<int> pares2 = nums.Where(n => n % 2 == 0).ToList();

int[] arrayNums = [3, 5, 6, 12, 7, 3, 8, 9, 23, 54];

// Los métodos Linq son comunes a todas las colecciones
Console.WriteLine(nums.All(n => n < 100));
Console.WriteLine(arrayNums.All(n => n < 100));

var masNums = nums.Concat([1, 2]);
List<int> masNums2 = [..nums, 1, 2];
Console.WriteLine(string.Join(",", masNums)); // 3,5,6,12,7,3,8,9,23,54,1,2
Console.WriteLine(string.Join(",", masNums2)); // 3,5,6,12,7,3,8,9,23,54,1,2

Console.WriteLine(nums.Average()); // 13
Console.WriteLine(string.Join(",", nums.Distinct())); // 3,5,6,12,7,8,9,23,54

var contarPares = nums.CountBy(n => n % 2 == 0);
foreach(var pair in contarPares)
{
  Console.WriteLine($"{(pair.Key ? "Pares" : "Impares")}: {pair.Value}");
}

Console.WriteLine(nums.First()); // 3
Console.WriteLine(nums.First(n => n % 2 == 0)); // 6

Console.WriteLine(nums.Max()); // 54

var numeros = Enumerable.Range(1, 10);
Console.WriteLine(string.Join(",", numeros));

var unos = Enumerable.Repeat(1, 10);
Console.WriteLine(string.Join(",", unos));

Console.WriteLine("Colección con 10 números aleatorios");
var aleatorios = Enumerable.Repeat(0, 10).Select(n => new Random().Next() % 100);
Console.WriteLine(string.Join(",", aleatorios));

var avg = nums.Where(n => n > 10).Average();
Console.WriteLine("Media números mayores a 10: " + avg);

var saltarMientrasMenorASeis = nums.SkipWhile(n => n < 6);
Console.WriteLine(string.Join(",", saltarMientrasMenorASeis));

var primerosMenoresASeis = nums.TakeWhile(n => n < 6);
Console.WriteLine(string.Join(",", primerosMenoresASeis));

// Ejemplos con personas
Console.WriteLine("-------- Ejemplos con personas -----");

var personas = new List<Persona>{
  new("Pepe", 23),
  new("Juan", 43),
  new("Ana", 14),
  new("Paco", 58),
  new("María", 37),
  new("Carlos", 16),
  new("María", 36),
  new("Juan", 32),
};

Console.WriteLine($"Media edades: {personas.Average(p => p.Edad):F2}");
var contarNombres = personas.CountBy(p => p.Nombre);
foreach(var pair in contarNombres)
{
  Console.WriteLine($"{pair.Key} aparece {pair.Value} veces");
}

Console.WriteLine(personas.Max(p => p.Edad)); // 58

var comparadorEdad = Comparer<Persona>.Create((p1, p2) => p1.Edad.CompareTo(p2.Edad));
var personasOrdenadas = personas.Order(comparadorEdad);
Console.WriteLine(string.Join(" - ", personasOrdenadas));

var personasOrdenadas2 = personas.OrderBy(p => p.Edad);
Console.WriteLine(string.Join(" - ", personasOrdenadas2));

var listaNombres = personas.Select(p => p.Nombre);
Console.WriteLine(string.Join(", ", listaNombres));

// Ordenar por nombre, y a igual nombre, por edad
var personasOrdenadasNombre = personas.OrderBy(p => p.Nombre).ThenBy(p => p.Edad);
Console.WriteLine(string.Join(", ", personasOrdenadasNombre));

var mayoresEdad = personas.Where(p => p.Edad >= 18).ToList();
Console.WriteLine(string.Join(", ", mayoresEdad));


