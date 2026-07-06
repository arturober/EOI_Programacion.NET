var animal = new Animal("Algo", 4);
animal.Comer();

var loro = new Ave("Loro", 1, true);
loro.Comer();

var leon = new Mamifero("León", 170, true);
leon.Comer();

var gacela = new Mamifero("Gacela", 90, false);
gacela.Comer();

//---------------------
var gerente = new Gerente(3);
gerente.Ascender();
Console.WriteLine(gerente.NombreRango);
