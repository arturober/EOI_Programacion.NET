var traducciones = new Dictionary<string, string>
{
  { "silla", "chair" },
  { "mesa", "table" },
  { "puerta", "door" },
  { "casa", "house" },
  { "árbol", "tree" },
  { "comida", "food" }
};

traducciones.Remove("árbol");

traducciones.Add("ordenador", "computer");

Console.WriteLine(traducciones.Count); // 5
Console.WriteLine(traducciones["mesa"]); // table
traducciones["ordenador"] = "laptop"; // Modificar valor
// traducciones.Add("ordenador", "computer"); // ERROR: An item with the same key has already been added. Key: ordenador
Console.WriteLine(string.Join(", ", traducciones));

foreach (var palabra in traducciones.Keys)
{
  Console.WriteLine($"{palabra} -> {traducciones[palabra]}");
}

Console.WriteLine(traducciones.ContainsKey("silla"));
Console.Write("Palabra a traducir: ");
string buscar = Console.ReadLine()!;
Console.WriteLine(traducciones.GetValueOrDefault(buscar, "Palabra no encontrada"));
traducciones.Remove("ordenador"); // Borramos la clave "ordenador"

/** Ejemplo gestión de tareas **/
var agenda = new Dictionary<DateTime, Tarea>
{
  { DateTime.Now.AddDays(3), new Tarea("Ir a comprar") },
  { DateTime.Parse("12/10/2026"), new Tarea("Cumpleaños de mi mapache") },
  { DateTime.Now.AddHours(5), new Tarea("Sacar la basura") }
};
foreach (var pair in agenda)
{
  Console.WriteLine($"{pair.Key} -> {pair.Value}");
}

