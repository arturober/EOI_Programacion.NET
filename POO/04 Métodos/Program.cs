/***********************************************************************************
* Métodos de objetos (métodos de instancia)
***********************************************************************************/

var producto = new Producto { Nombre = "Silla", Precio = 45.34 };
var producto2 = new Producto { Nombre = "Mesa", Precio = 102.32 };

Console.WriteLine(producto.GetPrecioImpuesto());
Console.WriteLine(producto2.GetPrecioImpuesto(0.1));

/*** EJERCICIOS 1 y 2 (parte 1) ***/

var persona = new Persona("Juan", 23);
persona.Saluda();
persona.Edad = -100; // No debería cambiar (negativo)
persona.Saluda();

