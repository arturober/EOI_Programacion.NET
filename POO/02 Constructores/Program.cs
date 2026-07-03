var producto = new Producto("Silla", 45.34);
Console.WriteLine(producto.Nombre);
Console.WriteLine(producto.Precio);
var producto2 = new Producto("Mesa", 102.3);

var persona = new Persona("Juan", 35);
var persona2 = new Persona("Pepe", 37) { FechaNac = DateOnly.Parse("13/04/1989") };

var cuadrado = new Cuadrado(23);
Console.WriteLine($"Cuadrado: Lado ({cuadrado.Lado}), área ({cuadrado.Area})");
var cuadrado2 = new Cuadrado();
Console.WriteLine($"Cuadrado: Lado ({cuadrado2.Lado}), área ({cuadrado2.Area})");
