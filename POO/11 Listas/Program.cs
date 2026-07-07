List<int> numeros = [1, 2, 3, 4, 5];
Console.WriteLine(string.Join(", ", numeros));
Console.WriteLine(numeros[3]); // 4
Console.WriteLine(numeros.Count); // 5

List<List<int>> matriz = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

// Recorrer Listas
Console.WriteLine("--------- Recorrer Listas --------");
// for(int i = 0; i < numeros.Count; i++)
// {
//   Console.WriteLine(numeros[i]);
// }
// foreach(int num in numeros)
// {
//   Console.WriteLine(num);
// }
// var enumerable = numeros.GetEnumerator();
// while(enumerable.MoveNext())
// {
//   Console.WriteLine(enumerable.Current);
// }
numeros.ForEach(Console.WriteLine);

// Métodos de listas
Console.WriteLine("--------- Añadir Elemento --------");
numeros.Add(6);
Console.WriteLine(string.Join(", ", numeros));
numeros.Insert(0, 888);
Console.WriteLine(string.Join(", ", numeros));

Console.WriteLine("--------- Concatenar listas --------");
List<int> numeros2 = [10, 20, 30, 40];
numeros.AddRange(numeros2);
Console.WriteLine(string.Join(", ", numeros)); // 888, 1, 2, 3, 4, 5, 6, 10, 20, 30, 40

List<string> palabras = ["mesa", "casa", "ratón"];
List<string> palabras2 = ["zanahoria", "lámpara", "puerta"];
// Concatenación con spread (no modifica la lista original)
List<string> palabras3 = [.. palabras, .. palabras2];
Console.WriteLine(string.Join(", ", palabras3)); // mesa, casa, ratón, zanahoria, lámpara, puerta

Console.WriteLine("--------- Borrar elementos --------");
numeros2.Clear();
Console.WriteLine(string.Join(", ", numeros2)); // (vacío)

palabras3.Remove("zanahoria");
Console.WriteLine(string.Join(", ", palabras3)); // mesa, casa, ratón, lámpara, puerta

palabras3.RemoveAt(2); // Borramos ratón
Console.WriteLine(string.Join(", ", palabras3)); // mesa, casa, lámpara, puerta

numeros.RemoveAll(n => n % 2 == 0); // Borramos los elementos pares
Console.WriteLine(string.Join(", ", numeros)); // 1, 3, 5

Console.WriteLine("--------- Buscar elementos --------");
List<string> nombres = ["Paco", "Ana", "María", "Alfredo", "Manuel", "Teodoro"];
Console.WriteLine(nombres.IndexOf("Manuel")); // 4
Console.WriteLine(nombres.IndexOf("Pedro")); // -1
Console.WriteLine(nombres.Contains("Manuel")); // true

Console.WriteLine(nombres.Find(n => n.StartsWith('A'))); // Ana
Console.WriteLine(nombres.FindLast(n => n.StartsWith('A'))); // Alfredo

List<string> nombresA = nombres.FindAll(n => n.StartsWith('A'));
Console.WriteLine(string.Join(", ", nombresA)); // Ana, Alfredo

Console.WriteLine("--------- Transformar elementos --------");
List<string> nombresMayus = nombres.ConvertAll(n => n.ToUpper()); // No modifica la lista original
Console.WriteLine(string.Join(", ", nombresMayus)); // PACO, ANA, MARÍA, ALFREDO, MANUEL, TEODORO

List<int> longitudes = nombres.ConvertAll(n => n.Length);
Console.WriteLine(string.Join(", ", longitudes)); // 4, 3, 5, 7, 6, 7

Console.WriteLine("--------- Rangos --------");
List<string> nombresSub = nombres.GetRange(2, 2);
Console.WriteLine(string.Join(", ", nombresSub)); // María, Alfredo
Console.WriteLine(string.Join(", ", nombres[2..4])); // María, Alfredo

Console.WriteLine("--------- Ordenar --------");
nombres.Sort((n1, n2) => n1.Length - n2.Length);
Console.WriteLine(string.Join(", ", nombres)); // Ana, Paco, María, Manuel, Alfredo, Teodoro
nombres.Reverse();
Console.WriteLine(string.Join(", ", nombres)); // Teodoro, Alfredo, Manuel, María, Paco, Ana

Console.WriteLine("--------- Comprobaciones --------");
Console.WriteLine(numeros.TrueForAll(n => n % 2 == 1)); // True
Console.WriteLine(nombres.TrueForAll(n => n.StartsWith('A'))); // False
Console.WriteLine(nombres.Any(n => n.StartsWith('A'))); // True

// EJERCICIO 1
Console.WriteLine("--------- Ejercicio 1 --------");

List<int> listaNumero = [10, 20, 30, 40];
listaNumero.AddRange(50,60);
listaNumero.Remove(20);
listaNumero.InsertRange(1, 25, 26);
listaNumero.Reverse();
Console.WriteLine(string.Join(",", listaNumero));


