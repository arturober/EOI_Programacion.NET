/***********************************************************************************
* Modificador static
***********************************************************************************/

Console.WriteLine(Persona.NumPersonas);
var p = new Persona("Paco", 35);
var p2 = Persona.PersonaGenerica();
Console.WriteLine(Persona.NumPersonas);
Persona.MostrarEstadisticas();

