var persona = new Persona("Tomás", 54);
persona.Hablar();
var cuervo = new Ave("Cuervo", 1.2, true);
cuervo.Hablar();

IHablador[] habladores = [
  persona,
  cuervo,
  new Mamifero("Koala", 6.7, false),
  new Persona("Pedro", 34),
];

foreach(IHablador hablador in habladores)
{
  hablador.Hablar();
}

Console.WriteLine("--------- Ejercicios 5 y 7 -----------");
IFigura[] figuras = [
  new Cuadrado(4.5),
  new Circulo(3),
  new Circulo(6.5),
  new Cuadrado(3.67),
  new Cuadrado(8),
  new Circulo(6.1)
];

foreach(IFigura figura in figuras)
{
  Console.WriteLine($"{figura.GetType()} -> Área: {figura.Area:F2}, perímetro: {figura.Perimetro:F2}");
}

object o = new Persona("Juan", 23);
Console.WriteLine(o.ToString()); // Juan, 23 años
Persona p = (Persona)o; // Casting explícito a Persona
p.Saluda();
