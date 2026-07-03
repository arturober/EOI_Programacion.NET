var persona = new Persona("Marco", 34);
var persona2 = new Persona("Marco", 34);
Console.WriteLine(persona);
Console.WriteLine(persona == persona2); // True al ser records (compara los valores y no la dirección de memoria)

var persona3 = persona;
persona3 = persona3 with { Nombre = "Pepito" }; // Clonamos objeto con nombre cambiado
Console.WriteLine(persona); // Persona { Nombre = Marco, Edad = 34 }
Console.WriteLine(persona3); // Persona { Nombre = Pepito, Edad = 34 }

var personaConDireccion = new Persona("Pedro", 24, new Direccion("Calle Nada", "23423", 23));
Console.WriteLine(personaConDireccion);
// Modificar la dirección (record anidado)
personaConDireccion = personaConDireccion with {
  Direccion = personaConDireccion.Direccion != null ? personaConDireccion.Direccion with { Numero = 13 } : null
};

Console.WriteLine(personaConDireccion);
