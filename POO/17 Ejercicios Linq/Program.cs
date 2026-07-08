// Colección de prueba utilizando expresiones de colección []
List<Empleado> empleados = [
  new Empleado("Ana", 24, "IT", 2500),
  new Empleado("Luis", 19, "Ventas", 1800),
  new Empleado("Carlos", 32, "IT", 3500),
  new Empleado("Marta", 28, "Marketing", 2200),
  new Empleado("Andrés", 24, "IT", 2100),
  new Empleado("Sofía", 28, "Ventas", 2500),
  new Empleado("Elena", 40, "RRHH", 3000),
  new Empleado("Pedro", 22, "Ventas", 2500),
  new Empleado("Juan", 21, "IT", 1900),
];

var ej7 = empleados
  .Where(e => e.Departamento == "IT" && e.Salario >= 2000)
  .OrderBy(e => e.Edad)
  .Select(e => e.Nombre)
  .Take(2);

Console.WriteLine(string.Join(", ", ej7));

var ej8 = empleados
    .Where(e => e.Departamento == "Ventas") // Filtra solo el equipo de Ventas
    .OrderByDescending(e => e.Salario) // Ordena por salario de mayor a menor
    .ThenBy(e => e.Edad) // Desempata: menor edad primero
    .FirstOrDefault();

if (ej8 != null)
{
  Console.WriteLine(ej8);
}

string[] exclusiones = ["RRHH", "Marketing"];

string[] ej9 = empleados
    .Select(e => e.Departamento) // Extrae todos los nombres de departamentos
    .Distinct()                         // Elimina los duplicados ("IT" y "Ventas" solo aparecen una vez)
    .Except(exclusiones)                // Resta los departamentos que estén en el array de exclusión
    .ToArray();                         // Convierte el flujo final en un Array

// Resultado esperado: "IT", "Ventas"
Console.WriteLine(string.Join(", ", ej9));

List<double> salarios = [2500, 1800, 3500, 2200, 2100, 2500, 3000, 2500];

double ej10 = empleados
  .Select(e => e.Salario)
  .OrderByDescending(s => s) // 1. Ordena: 3500, 3000, 2500, 2500, 2500, 2200, 2100, 1800
  .SkipWhile(s => s > 2800)  // 2. Descarta consecutivamente los mayores a 2800 (quita 3500 y 3000)
  .Skip(2)                          // 3. Salta los 2 primeros del bloque restante (quita los dos primeros 2500)
  .Take(3)                          // 4. Captura los 3 siguientes: el último 2500, 2200 y 2100
  .Average();                       // 5. Calcula la media de (2500 + 2200 + 2100) / 3

// Resultado esperado: 2266.66
Console.WriteLine($"Media: {ej10:F2}");
