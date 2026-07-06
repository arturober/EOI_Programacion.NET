/***********************************************************************************
* Modificador static
***********************************************************************************/

Console.WriteLine(Persona.NumPersonas);
var paco = new Persona("Paco", 35);
var marta = new Persona("Marta", 23);
var generica = Persona.PersonaGenerica();
Console.WriteLine(Persona.NumPersonas);
Persona.MostrarEstadisticas();

/****** Ejercicio 7 ********/
Persona[] personas = [
  generica,
  new Persona("María", 25),
  paco,
  new Persona("Sara", 53),
];

Console.WriteLine($"Paco está en el array: {paco.EstoyEnArray(personas)}");
Console.WriteLine($"Marta está en el array: {marta.EstoyEnArray(personas)}");
