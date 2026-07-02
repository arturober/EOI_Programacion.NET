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

string ruta = Path.Join("archivos", "prueba.txt");
// LeerFichero(ruta);
// LeerFichero2(ruta);
// LeerFichero3(ruta);
// LeerFichero4(ruta);
EscribirFichero(Path.Join("archivos", "a1.txt"));
EscribirFichero2(Path.Join("archivos", "a2.txt"));
EscribirFichero3(Path.Join("archivos", "a3.txt"));
Console.WriteLine("Programa terminado");

