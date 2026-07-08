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


