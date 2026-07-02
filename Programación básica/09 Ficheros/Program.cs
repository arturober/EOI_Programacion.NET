char sep = Path.DirectorySeparatorChar;

string ruta1 = $"dir1{sep}dir2{sep}archivo.txt";
Console.WriteLine(ruta1);
string ruta2 = string.Join(sep, ["dir1", "dir2", "archivo.txt"]);
Console.WriteLine(ruta2);
string ruta3 = Path.Join("dir1", "dir2", "archivo.txt");
Console.WriteLine(ruta3);

/**
* Lectura clásica de un fichero línea a línea
*/
void LeerFichero(string ruta)
{
  string? line;
  var file = new StreamReader(ruta); // Abrimos archivo en modo lectura
  while ((line = file.ReadLine()) != null)
  {
    Console.WriteLine(line);
  }
  file.Close();
}

/**
* Lectura asíncrona de un fichero línea a línea
*/
async void LeerFichero2(string ruta)
{
  await foreach (string line in File.ReadLinesAsync(ruta))
  {
    Console.WriteLine(line);
  }
}

/**
* Leer todo el contenido del archivo de golpe en un string
*/
async void LeerFichero3(string ruta)
{
  string texto = File.ReadAllText(ruta);
  Console.WriteLine(texto);
}

/**
* Leer todo el contenido del archivo y guardarlo en un array de strings
*/
async void LeerFichero4(string ruta)
{
  string[] lineas = File.ReadAllLines(ruta);
  foreach (var linea in lineas)
  {
    Console.WriteLine(linea);
  }
}

/**
* Escribir el contenido de un string en un fichero
*/
void EscribirFichero(string ruta)
{
  string texto = """
    Contenido del archivo
    con varias líneas.
    """;

  File.WriteAllText(ruta, texto);
}

/**
* Escribir el contenido de una colección de strings en un fichero
*/
void EscribirFichero2(string ruta)
{
  string[] lineas = [
    "Línea 1",
    "Línea 2",
    "Línea 3"
  ];
  File.WriteAllLinesAsync(ruta, lineas);
}

/**
* Escribir el contenido de una colección de strings en un fichero
*/
void EscribirFichero3(string ruta)
{
  string[] lineas = [
    "Línea 1",
    "Línea 2",
    "Línea 3"
  ];
  var writer = new StreamWriter(ruta, true);
  foreach (string linea in lineas)
  {
    writer.WriteLine(linea);
  }
  writer.Close();
}

void Ejercicio1(string ruta)
{
  string[] lineas = File.ReadAllLines(ruta);
  int suma = 0;
  foreach (var linea in lineas)
  {
    suma += int.Parse(linea);
  }
  Console.WriteLine($"Total: {suma}");
}

void Ejercicio1bis(string ruta)
{
  int suma = File.ReadAllLines(ruta).Sum(int.Parse);
  Console.WriteLine($"Total: {suma}");
}

void Ejercicio3(string ruta)
{
  string[] lineas = File.ReadAllLines(ruta);
  double notaMin = 10, notaMax = 0, media = 0;
  string nombreMin = "", nombreMax = "";
  foreach (var linea in lineas)
  {
    string[] datos = linea.Split(";");
    string nombre = datos[0];
    double nota = double.Parse(datos[1]);
    media += nota;
    if(nota < notaMin)
    {
      notaMin = nota;
      nombreMin = nombre;
    }
    if(nota > notaMax)
    {
      notaMax = nota;
      nombreMax = nombre;
    }
  }
  media /= lineas.Length;
  Console.WriteLine($"Media: {media}");
  Console.WriteLine($"Nota mínima: {nombreMin} -> {notaMin}");
  Console.WriteLine($"Nota máxima: {nombreMax} -> {notaMax}");
}

void Ejercicio3bis(string ruta)
{
  string[] lineas = File.ReadAllLines(ruta);
  string[] nombres = new string[lineas.Length];
  double[] notas = new double[lineas.Length];
  for (int i = 0; i < lineas.Length; i++)
  {
    string[] datos = lineas[i].Split(";");
    nombres[i] = datos[0];
    notas[i] = double.Parse(datos[1]);
  }
  Console.WriteLine($"Media: {notas.Average()}");
  double notaMin = notas.Min();
  double notaMax = notas.Max();
  int indexMin = notas.IndexOf(notaMin);
  int indexMax = notas.IndexOf(notaMax);
  Console.WriteLine($"Nota mínima: {nombres[indexMin]} -> {notaMin}");
  Console.WriteLine($"Nota máxima: {nombres[indexMax]} -> {notaMax}");
}

string ruta = Path.Join("archivos", "prueba.txt");
// LeerFichero(ruta);
// LeerFichero2(ruta);
// LeerFichero3(ruta);
// LeerFichero4(ruta);
// EscribirFichero(Path.Join("archivos", "a1.txt"));
// EscribirFichero2(Path.Join("archivos", "a2.txt"));
// EscribirFichero3(Path.Join("archivos", "a3.txt"));
// Ejercicio1(Path.Join("archivos", "numeros.txt"));
// Ejercicio1bis(Path.Join("archivos", "numeros.txt"));
// Ejercicio3(Path.Join("archivos", "notas.txt"));
Ejercicio3bis(Path.Join("archivos", "notas.txt"));

int[] nums = [23, 43, 65, 12, 87, 3, 7];
int min = nums.Min();
int minIndex = nums.IndexOf(min);
System.Console.WriteLine();
Console.WriteLine("Programa terminado");

