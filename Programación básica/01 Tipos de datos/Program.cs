// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");
Console.WriteLine("Me llamo Arturo");

int num = 23; // Variable
num = 13; // Reasignación de valor

const double PI = 3.1416; // Constante. No se puede cambiar el valor

Console.WriteLine(num); // 13

int num2 = num / 3;
Console.WriteLine(num2); // 4
Console.WriteLine(num % 3); // 1
Console.WriteLine(num / 3.0); // 4,33333333

string nombre = "Pepe";
Console.WriteLine(nombre); // Pepe
Console.WriteLine("Hola " + nombre); // Pepe

double precio = 2.32;
Console.WriteLine(precio + "€");

bool cierto = false;
Console.WriteLine("¿Es cierto?: " + cierto);

// double: el tipo decimal más común
double pi = 3.141592653589793;
double altura = 1.75;

// float: menos precisión, necesita la "f" al final
float velocidad = 120.5f;

// decimal: alta precisión para dinero (necesita la "m" al final)
decimal salario = 2500.50m;

// Sin interpolación
Console.WriteLine("Pi: " + pi);

// Con interpolación
Console.WriteLine($"Pi: {pi}");
Console.WriteLine($"Altura: {altura} metros");
Console.WriteLine($"Velocidad: {velocidad} km/h");
Console.WriteLine($"Precio: {precio}€");
Console.WriteLine($"Salario: {salario}€");

// Cadena multilínea
string menu = """
    ╔══════════════════════════╗
    ║    MENÚ PRINCIPAL        ║
    ╠══════════════════════════╣
    ║  1. Nuevo juego          ║
    ║  2. Cargar partida       ║
    ║  3. Opciones             ║
    ║  4. Salir                ║
    ╚══════════════════════════╝
    """;
Console.WriteLine(menu);

// Cadena multilínea
string comentario = """
Lo primero que me llamó la atención fue lo ligeros que son estos auriculares, tienen un peso de 240 gramos. 🎧 Me llamó la atención, ya que se sienten cómodos después de varias horas usándolos. Son perfectos para esas largas sesiones de juego o trabajo donde normalmente terminas con las orejas cansadas.

Las orejeras son muy cómodas y suaves,, al igual que l diadema viene con un acolchado de espuma viscoelástica súper suave, que se adapta muy bien a la cabeza y se siente cómoda, sin que me presione demasiado, lo cual es un alivio. 🙌

El sonido es bueno. Viene con diafragmas de 50 mm realmente se notan. Cuando juego, se puede apreciar de dónde vienen los pasos o los disparos, muy útil para juegos de disparos como los shooters. 🎮 Además, vienen con un sonido envolvente 7.1, es súper inmersivo, hace que te sientas totalmente dentro del juego o la película que estoy viendo. Los graves son potentes y los agudos muy nítidos, me encanta la claridad del audio.

El micrófono también es otro detalle que me ha gustado mucho. 🎤 Se escucha muy claro y la cancelación de ruido pasiva ayuda a que solo se me oiga a mí, sin ruidos de fondo molestos. Además, es flexible, así que puedo ajustarlo a la posición que prefiera sin problemas. Esto es genial si estás en una partida online y necesitas que tu equipo te escuche perfectamente. Lo que no me gustó del micrófono, es que no se puede quitar, viene fijo, al igual que el cable de audio, que no se puede cambiar.
""";
Console.WriteLine(comentario);