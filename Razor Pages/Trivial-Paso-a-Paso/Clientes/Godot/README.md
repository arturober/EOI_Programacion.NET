# Cliente del trivial con Godot 4.7.1 y C#

Este proyecto es un cliente gráfico para la API de trivial.

La escena contiene únicamente un `Control` a pantalla completa. Todos los controles, estilos,
eventos y peticiones HTTP se crean mediante el archivo `JuegoTrivial.cs`.

No es necesario:

- Crear botones desde el editor.
- Crear etiquetas desde el editor.
- Añadir contenedores manualmente.
- Configurar señales desde el panel Node.
- Añadir imágenes, fuentes o estilos externos.
- Diseñar menús.

## Requisitos

- Godot 4.7.1 con soporte para .NET.
- SDK de .NET 8 o posterior.
- La versión REST de la API de trivial ejecutándose.

Debe utilizarse la descarga de Godot que incluye `.NET`, no la versión
estándar.

La API puede seguir estando desarrollada con .NET 10. El cliente tiene como
destino `net8.0`, que es compatible con Godot 4.7.1. Se comunican mediante HTTP,
por lo que el cliente y el servidor no necesitan utilizar la misma versión de
.NET.

## Versión necesaria de la API

Este cliente consume estas dos operaciones:

```text
GET /api/categorias
GET /api/preguntas?categoriaId=2&cantidad=10
```

Por tanto, hay que ejecutar la versión `05-API-REST` o una posterior del
proyecto progresivo. Las versiones `05`, `06` y `07` ya contienen
`CategoriasController`, `PreguntasController` y sus DTO.

El `Program.zip` utilizado durante las pruebas de Razor Pages no publica aún
esas rutas. Su `Program.cs` solamente contiene `AddRazorPages` y
`MapRazorPages`, por lo que el cliente recibiría un error 404 si se ejecutase
esa versión aislada.

Para que una aplicación ASP.NET Core con los controladores REST los publique,
`Program.cs` debe registrar y mapear los controladores:

```csharp
builder.Services.AddControllers();

// Resto de la configuración...

app.MapControllers();
```

Además, deben existir los controladores y DTO incluidos desde la versión 05.

## Estructura

```text
TrivialGodotCSharp
├── .gitignore
├── JuegoTrivial.cs
├── Main.tscn
├── README.md
├── TrivialGodotCSharp.csproj
└── project.godot
```

## La escena

`Main.tscn` contiene únicamente:

```text
JuegoTrivial (Control)
└── JuegoTrivial.cs
```

Los controles que aparecen durante la ejecución son hijos creados por código.
No están almacenados en la escena.

## Ejecutar

1. Inicia primero la API:

```bash
dotnet run
```

2. Comprueba la dirección mostrada por la API, por ejemplo:

```text
http://localhost:5000
```

3. Abre `project.godot` con Godot 4.7.1 .NET.
4. Pulsa F6 o F5.
5. Escribe la dirección de la API.
6. Pulsa **Conectar y cargar categorías**.
7. Elige una categoría.
8. Pulsa **Comenzar partida**.

## Conectar con MonsterASP.NET

El cliente Godot no se publica dentro de `/wwwroot`. Primero se despliega
`TrivialApi` de la versión 5, 6 o 7 y después Godot se ejecuta en el equipo
del usuario.

En el campo de dirección escribe, por ejemplo:

```text
https://tu-sitio.runasp.net
```

No añadas `/api`: el cliente completa internamente las rutas
`/api/categorias` y `/api/preguntas`.

Comprueba previamente:

```text
https://tu-sitio.runasp.net/api/categorias
```

La aplicación Godot nativa no está sometida a CORS. Este proyecto utiliza C# y
Godot .NET 4.7.1, por lo que no puede exportarse para web; está pensado para
escritorio o plataformas compatibles con Godot .NET.

## Funcionalidad

- Dirección del servidor modificable.
- Conexión con la API.
- Descarga de categorías.
- Selección de una categoría o de todas.
- Diez preguntas aleatorias.
- Cuatro botones de respuesta.
- Indicador de progreso.
- Contador de aciertos.
- Presentación de la respuesta correcta al fallar.
- Resultado final.
- Posibilidad de jugar otra partida.
- Interfaz adaptable a distintos tamaños de ventana.

## Flujo de comunicación

```text
Godot
  ↓ HTTP GET
/api/categorias
  ↓
Selector de categorías
  ↓ HTTP GET
/api/preguntas?cantidad=10
  ↓
Juego
```

Si se selecciona una categoría concreta, la segunda dirección será parecida a:

```text
/api/preguntas?categoriaId=2&cantidad=10
```

## `_Ready`

Godot ejecuta `_Ready` cuando el `Control` entra en la escena:

```csharp
public override void _Ready()
{
    CrearInterfaz();
    _http = new HttpRequest();
    AddChild(_http);
}
```

Se crea:

- El nodo encargado de HTTP.
- Toda la interfaz.

## Interfaz programada

`CrearInterfaz` construye:

```text
Control
├── ColorRect
└── MarginContainer
    └── ScrollContainer
        └── PanelContainer
            └── MarginContainer
                └── VBoxContainer
```

Dentro del último `VBoxContainer` se crean tres pantallas:

- Inicio.
- Juego.
- Resultado final.

Cambiar de pantalla consiste únicamente en modificar `Visible`.

## HttpRequest

Godot proporciona el nodo `HttpRequest`, por lo que no se instala ningún
paquete.

La petición comienza con:

```csharp
Error inicioPeticion = _http.Request(url);
```

Después se espera su señal:

```csharp
Variant[] respuesta = await ToSignal(
    _http,
    HttpRequest.SignalName.RequestCompleted);
```

`await` mantiene la aplicación funcionando mientras llega la respuesta.

## Código HTTP

El segundo dato de la señal contiene el código:

```csharp
long codigoHttp = respuesta[1].AsInt64();
```

Los códigos correctos están entre 200 y 299.

## Convertir el cuerpo a JSON

El cuarto dato contiene los bytes:

```csharp
byte[] cuerpo = respuesta[3].AsByteArray();
```

Se convierten a texto:

```csharp
string json = Encoding.UTF8.GetString(cuerpo);
```

Y después al DTO:

```csharp
JsonSerializer.Deserialize<T>(json, _opcionesJson);
```

## DTO

Los DTO reproducen la estructura pública de la API:

```csharp
public sealed class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}
```

`PreguntaDto` contiene:

- Id.
- Enunciado.
- Array de respuestas.
- Número de respuesta correcta.
- Categoría.

Estos objetos no utilizan Entity Framework.

## Estado de la partida

```csharp
private List<PreguntaDto> _preguntas = [];
private int _posicion;
private int _aciertos;
```

- `_preguntas` almacena el JSON convertido.
- `_posicion` indica la pregunta actual.
- `_aciertos` almacena el marcador.

## Crear respuestas

Para cada respuesta se crea un botón:

```csharp
Button boton = CrearBoton(...);
boton.Pressed += () => Responder(numeroRespuesta);
```

No se conectan señales desde el editor.

## Comprobar una respuesta

La API numera las respuestas del 1 al 4:

```csharp
numeroRespuesta == pregunta.RespuestaCorrecta
```

Los arrays empiezan en cero. Para obtener el texto correcto se resta uno:

```csharp
pregunta.Respuestas[
    pregunta.RespuestaCorrecta - 1
]
```

## Conexión desde otro dispositivo

`localhost` siempre significa “este mismo dispositivo”.

Si el cliente Godot se ejecuta en otro ordenador o móvil, debe escribirse la
dirección IP del ordenador que ejecuta la API:

```text
http://192.168.1.50:5000
```

También será necesario:

- Hacer que la API escuche conexiones de la red.
- Permitir el puerto en el firewall.

## CORS

Una aplicación Godot nativa no está sujeta a la política CORS de los
navegadores. Puede consumir la API aunque no se habilite CORS.

## Exportación web

Los proyectos C# de Godot 4.7 no pueden exportarse a Web. Este cliente está
pensado para Windows, Linux, macOS o las plataformas móviles compatibles con
Godot .NET.

Para un cliente web debe utilizarse GDScript o mantener el cliente HTML y
JavaScript de la API.

## Errores frecuentes

### No se puede conectar

Comprueba:

- Que la API esté ejecutándose.
- Que el puerto sea correcto.
- Que hayas escrito `http://` o `https://`.
- Que `/api/categorias` funcione en el navegador.

### Godot no reconoce C#

Has abierto el proyecto con la versión estándar. Descarga Godot 4.7.1 .NET.
La versión correcta se identifica como **Godot Engine .NET** en la ventana
de bienvenida. Con la versión estándar el script C# no se ejecuta.

### La ventana aparece completamente gris

1. Comprueba que has abierto el proyecto con **Godot 4.7.1 .NET**, no con la
   descarga estándar.
2. Pulsa **Compilar** en la esquina superior derecha del editor.
3. Abre el panel **Salida** y verifica que no haya errores de C#.
4. Si acabas de instalar .NET, cierra y vuelve a abrir Godot.

La escena utiliza un nodo raíz `Control` a pantalla completa. Si el script se
compila, debe aparecer inmediatamente el fondo azul oscuro y el formulario,
aunque la API todavía no esté ejecutándose.

### No se encuentra el SDK

Instala el SDK de .NET 8 de 64 bits.

### HTTPS local falla

Durante las pruebas resulta más sencillo utilizar la dirección HTTP mostrada
por ASP.NET Core:

```text
http://localhost:5000
```

## Posibles ampliaciones

- Elegir el número de preguntas.
- Añadir un temporizador.
- Colorear la respuesta seleccionada.
- Guardar la mejor puntuación.
- Añadir sonidos generados por código.
- Permitir jugar de nuevo con la misma categoría.

## Fuentes oficiales

- C# en Godot 4.7:
  https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_basics.html
- Godot 4.7.1:
  https://godotengine.org/article/maintenance-release-godot-4-7-1/
- SDK de Godot para .NET 4.7.1:
  https://www.nuget.org/packages/Godot.NET.Sdk
