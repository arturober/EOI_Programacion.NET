Mamifero leon = new("León", 230, true);
Mamifero leon2 = new("León", 230, true);

Console.WriteLine(leon);
Console.WriteLine(leon == leon2); // True -> Los records comparan por valores por defecto
Console.WriteLine(leon.Equals(leon2)); // True -> Los records comparan por valores por defecto
Console.WriteLine(ReferenceEquals(leon, leon2)); // False -> no son el mismo objeto (referencia)

Mamifero leon3 = leon with { Peso = 250}; // Clonamos leon con peso diferente
Console.WriteLine(leon3);
