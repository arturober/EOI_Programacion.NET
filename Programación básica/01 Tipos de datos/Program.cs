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

// Secuencias de escape
Console.WriteLine("Primera línea\nSegunda línea");
Console.WriteLine("Nombre:\tAna");
Console.WriteLine("Ruta: C:\\Users\\Ana\\Documentos");

// Alternativa: cadena verbatim con @
string ruta = @"C:\Users\Ana\Documentos";  // No necesita \\
Console.WriteLine(ruta);

var edad2 = 25;              // El compilador deduce que es int
var nombre2 = "Ana";         // El compilador deduce que es string
var precio2 = 9.99;          // El compilador deduce que es double
var activo2 = true;          // El compilador deduce que es bool

// Es exactamente equivalente a:
int edad3 = 25;
string nombre3 = "Ana";
double precio3 = 9.99;
bool activo3 = true;


const int DIAS_SEMANA = 7;
const string MONEDA = "EUR";
const int MAYORIA_EDAD = 18;

Console.WriteLine($"Pi vale: {PI}");
Console.WriteLine($"Una semana tiene {DIAS_SEMANA} días");

// Pedir el nombre
Console.Write("¿Cómo te llamas? ");
string nombre4 = Console.ReadLine();

// Pedir la edad (ReadLine devuelve string, hay que convertir a int)
Console.Write("¿Cuántos años tienes? ");
string edadTexto = Console.ReadLine();
int edad = int.Parse(edadTexto);

// Pedir la altura
Console.Write("¿Cuánto mides (en metros)? ");
string alturaTexto = Console.ReadLine();
double altura4 = double.Parse(alturaTexto);

// Mostrar los datos
Console.WriteLine();
Console.WriteLine("═══════════════════════════");
Console.WriteLine($"  Nombre: {nombre4}");
Console.WriteLine($"  Edad: {edad} años");
Console.WriteLine($"  Altura: {altura4} m");
Console.WriteLine("═══════════════════════════");


Console.Write("Introduce un número: ");
string entrada = Console.ReadLine();

// TryParse devuelve true si la conversión fue exitosa
if (int.TryParse(entrada, out int numero))
{
    Console.WriteLine($"Has introducido el número: {numero}");
}
else
{
    Console.WriteLine("Eso no es un número válido.");
}

Console.WriteLine("No ha pasado nada");

double precio6 = 9.99;
int precioRedondeado = (int)precio6;  // Casting: se pierden los decimales
Console.WriteLine(precioRedondeado); // 9 (NO redondea, TRUNCA)

long numeroGrande = 1000;
int numeroNormal = (int)numeroGrande;  // Casting de long a int

// Cuidado con el desbordamiento:
int valor = 300;
byte valorByte = (byte)valor;  // ⚠️ 300 no cabe en byte (0-255)
Console.WriteLine(valorByte);  // 44 (resultado inesperado por desbordamiento)

int? edadDesconocida = null;   // El ? permite que sea null
double? peso = null;
bool? respuesta = null;        // No ha respondido

// Asignar un valor después
//edadDesconocida = 25;

// Comprobar si tiene valor
if (edadDesconocida.HasValue)
{
    Console.WriteLine($"Edad: {edadDesconocida.Value}");
}
else
{
    Console.WriteLine("Edad desconocida");
}

// Operador ?? (null-coalescing): valor por defecto si es null
int edadFinal = edadDesconocida ?? 0;  // Si es null, usa 0
Console.WriteLine($"Edad final: {edadFinal}");
