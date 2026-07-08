var traducciones = new SortedDictionary<string, string>
{
  { "silla", "chair" },
  { "mesa", "table" },
  { "puerta", "door" },
  { "casa", "house" },
  { "árbol", "tree" },
  { "comida", "food" }
};

foreach (var palabra in traducciones.Keys)
{
  Console.WriteLine($"{palabra} -> {traducciones[palabra]}");
}

var comparador = Comparer<IFigura>.Create((f1, f2) => f1.Area.CompareTo(f2.Area));

var figuras = new SortedSet<IFigura>(comparador)
{
  new Circulo(12),
  new Cuadrado(12),
  new Circulo(4),
  new Cuadrado(7.5),
};

foreach(IFigura figura in figuras)
{
  Console.WriteLine($"{figura.GetType()} -> Área: {figura.Area:F2}");
}

List<Persona> personas = [
  new Persona("Pedro", 24),
  new Persona("María", 35),
  new Persona("Juan", 52),
  new Persona("Bea", 28),
  new Persona("Alberto", 19),
];

var comparadorEdad = Comparer<Persona>.Create((p1,p2) => p1.Edad.CompareTo(p2.Edad));
var comparadorNombre = Comparer<Persona>.Create((p1,p2) => p1.Nombre.CompareTo(p2.Nombre));

var conjuntoPersonas1 = new SortedSet<Persona>(personas, comparadorEdad);
var conjuntoPersonas2 = new SortedSet<Persona>(personas, comparadorNombre);

Console.WriteLine("---- Personas ordenadas por edad ----");
foreach(var persona in conjuntoPersonas1)
{
  Console.WriteLine(persona);
}

Console.WriteLine("---- Personas ordenadas por nombre ----");
foreach(var persona in conjuntoPersonas2)
{
  Console.WriteLine(persona);
}
