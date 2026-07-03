/***********************************************************************************
* Constructor de copia
***********************************************************************************/

var persona = new Persona("Marta", 40)
{
  Direccion = new Direccion("Calle Perdida", "23444", 53),
  Telefonos =  ["9345345945", "6546456345"]
};

var persona2 = new Persona(persona); // Copiamos la persona
persona2.Edad = 23;
persona2.Telefonos[1] = "57456654565";

persona2.Direccion?.Numero = 17; // Cambiamos objeto interno (dirección)


persona.Saluda();
persona2.Saluda();

