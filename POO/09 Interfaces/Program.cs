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
