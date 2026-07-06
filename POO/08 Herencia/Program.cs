var loro = new Ave("Loro", 1, true);
loro.Comer();

var leon = new Mamifero("León", 170, true);
leon.Comer();

var gacela = new Mamifero("Gacela", 90, false);
gacela.Comer();

Console.WriteLine("--------- ToString -----------");

Console.WriteLine(loro);

Console.WriteLine("--------- Equals -----------");

var perro = new Mamifero("Perro", 24, true);
var perro2 = perro.Clone();

Console.WriteLine(perro);
Console.WriteLine(perro2);
Console.WriteLine(perro2.TipoAnimal());

Console.WriteLine(perro == perro2);
Console.WriteLine(perro.Equals(perro2));
Console.WriteLine(ReferenceEquals(perro, perro2));

Console.WriteLine("--------- Clonación en profundidad -----------");

Zoo zoo = new Zoo("Exploración animal", 20);
zoo.AddAnimal(new Mamifero("Cebra", 190, false));
Zoo zoo2 = zoo.Clone();
zoo2.GetAnimal(0).Peso = 999;
Console.WriteLine(zoo.GetAnimal(0));
Console.WriteLine(zoo2.GetAnimal(0));

//---------------------
var gerente = new Gerente(3);
gerente.Ascender();
Console.WriteLine(gerente.NombreRango);

// Ejercicios 1 y 2
var tiendaLicores = new TiendaLicores();
tiendaLicores.Bienvenida();
