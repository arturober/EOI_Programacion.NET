var producto = new Producto { Nombre = "Silla", Precio = 45.34 };
Console.WriteLine(producto.Nombre);
Console.WriteLine(producto.Precio);
var producto2 = new Producto { Nombre = "Mesa", Precio = 102.32 };

var persona = new Persona { Nombre = "Juan", Edad = 35 };

var cuadrado = new Cuadrado { Lado = 4.5 };
Console.WriteLine($"Cuadrado: Lado ({cuadrado.Lado}), área ({cuadrado.Area})");

