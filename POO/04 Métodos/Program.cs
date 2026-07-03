/***********************************************************************************
* Métodos de objetos (métodos de instancia)
***********************************************************************************/

var producto = new Producto { Nombre = "Silla", Precio = 45.34 };
var producto2 = new Producto { Nombre = "Mesa", Precio = 102.32 };

Console.WriteLine(producto.GetPrecioImpuesto());
Console.WriteLine(producto2.GetPrecioImpuesto(0.1));


