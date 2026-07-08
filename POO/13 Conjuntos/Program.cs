using System.Runtime.InteropServices;

var set = new HashSet<string>
{
  "Manzana",
  "Plátano",
  "Calabaza",
  "Manzana",
  "Plátano"
};

Console.WriteLine(set.Contains("Manzana")); // true
Console.WriteLine(set.Contains("Pera")); // false

set.Add("Tomate");
set.Add("Plátano"); // No lo inserta 2 veces (no admite repetidos)
set.Remove("Manzana");
foreach(string comida in set)
{
  Console.WriteLine(comida);
}

var personas = new HashSet<Persona>{
  new("Pepe", 23),
  new("Juan", 43),
  new("Ana", 14),
  new("Paco", 58),
};

Console.WriteLine(personas.Contains(new("Pepe", 23))); // true (usa equals para comparar). Son objetos diferentes

var tareas = new HashSet<Tarea>
{
  new("Sacar la basura", DateOnly.Parse("09/07/2026")),
  new("Salir a cenar", DateOnly.Parse("13/08/2026")),
  new("Buscar casa", DateOnly.Parse("25/07/2026")),
  new("Terminar estudios", DateOnly.Parse("17/04/2027")),
};

Console.WriteLine(tareas.Contains(new("Buscar casa", DateOnly.Parse("25/07/2026")))); // true. Los record implementan por defecto Equals
