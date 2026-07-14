# Ahorcado con C# y SQLite

Juego del **ahorcado para consola**, desarrollado en **C#** y conectado a una base de datos **SQLite**.

El proyecto está pensado como una práctica didáctica para estudiantes que están comenzando con C#, programación orientada a objetos y acceso a bases de datos. El código prioriza la claridad y la facilidad de comprensión por encima de la optimización.

## Descripción

El jugador debe adivinar una palabra letra a letra antes de alcanzar el número máximo de errores.

Las palabras y sus pistas se almacenan en una base de datos SQLite. Al terminar cada partida, el programa guarda el resultado y permite consultar las estadísticas del jugador.

La base de datos se crea automáticamente durante la primera ejecución, por lo que no es necesario preparar ningún fichero manualmente.

## Características

- Juego del ahorcado completamente funcional en consola.
- Palabras y pistas almacenadas en SQLite.
- Selección aleatoria de palabras.
- Validación de las letras introducidas.
- Control de letras repetidas.
- Dibujo progresivo del ahorcado.
- Registro de partidas ganadas y perdidas.
- Almacenamiento del número de errores y la fecha.
- Estadísticas personales por jugador.
- Creación automática de la base de datos y sus tablas.
- Código dividido en clases y ficheros.
- Consultas SQL parametrizadas.

## Tecnologías utilizadas

- C#
- .NET
- SQLite
- Microsoft.Data.Sqlite
- Programación orientada a objetos
- Aplicación de consola

## Requisitos

Para ejecutar el proyecto necesitas:

- [.NET SDK](https://dotnet.microsoft.com/download)
- Visual Studio, Visual Studio Code, Rider o cualquier editor compatible con C#

Puedes comprobar que .NET está instalado con:

```bash
dotnet --version
```

## Creación del proyecto

Para crear un proyecto nuevo desde la terminal:

```bash
dotnet new console -n AhorcadoSQLite
cd AhorcadoSQLite
dotnet add package Microsoft.Data.Sqlite
```

Después, copia los ficheros del proyecto dentro de la carpeta creada.

## Ejecución

Desde la carpeta del proyecto:

```bash
dotnet run
```

Durante la primera ejecución se creará automáticamente el fichero:

```text
ahorcado.db
```

Este fichero contiene las palabras, las pistas y el historial de partidas.

## Estructura del proyecto

```text
AhorcadoSQLite
├── Program.cs
├── Palabra.cs
├── Estadisticas.cs
├── BaseDatos.cs
├── JuegoAhorcado.cs
├── AhorcadoSQLite.csproj
└── README.md
```

### `Program.cs`

Es el punto de entrada de la aplicación.

Sus responsabilidades son:

1. Configurar la consola para mostrar correctamente los caracteres en español.
2. Crear el objeto encargado de la base de datos.
3. Preparar las tablas y los datos iniciales.
4. Crear el juego.
5. Iniciar la aplicación.

Este fichero no contiene reglas del juego ni instrucciones SQL.

### `Palabra.cs`

Representa una palabra obtenida de la base de datos.

Cada objeto contiene:

- El texto que debe adivinarse.
- Una pista para ayudar al jugador.

Esta clase permite transformar una fila de SQLite en un objeto de C# fácil de utilizar.

### `Estadisticas.cs`

Representa las estadísticas de un jugador.

Contiene:

- Número de partidas.
- Número de victorias.
- Número de derrotas.

Las derrotas se calculan restando las victorias al total de partidas, evitando almacenar información redundante.

### `BaseDatos.cs`

Contiene todo el código relacionado con SQLite.

Se encarga de:

- Abrir conexiones.
- Crear las tablas.
- Insertar las palabras iniciales.
- Obtener una palabra aleatoria.
- Guardar partidas.
- Consultar las estadísticas.

De esta forma, el resto del programa no necesita conocer las instrucciones SQL.

### `JuegoAhorcado.cs`

Contiene las reglas del juego y la interacción con el usuario.

Se encarga de:

- Pedir el nombre del jugador.
- Mostrar el estado de la partida.
- Solicitar y validar letras.
- Detectar letras repetidas.
- Contar los errores.
- Comprobar la victoria o la derrota.
- Solicitar a `BaseDatos` que guarde el resultado.
- Mostrar las estadísticas finales.

## Funcionamiento general

El flujo principal del programa es el siguiente:

```text
Program
   │
   ├── crea BaseDatos
   │       │
   │       ├── crea las tablas
   │       └── inserta las palabras iniciales
   │
   └── crea JuegoAhorcado
           │
           ├── pide el nombre del jugador
           ├── obtiene una palabra de SQLite
           ├── ejecuta la partida
           ├── guarda el resultado
           └── muestra las estadísticas
```

## Funcionamiento de una partida

1. El programa obtiene una palabra aleatoria de SQLite.
2. Se muestra una pista y una serie de guiones bajos.
3. El jugador introduce una letra.
4. El programa comprueba que:
   - Se haya escrito un único carácter.
   - El carácter sea una letra.
   - La letra no se haya utilizado anteriormente.
5. Si la letra aparece en la palabra, se muestran todas sus posiciones.
6. Si no aparece, aumenta el número de errores.
7. La partida termina cuando:
   - Se descubren todas las letras.
   - Se alcanza el máximo de errores.
8. El resultado se guarda en SQLite.
9. El jugador puede comenzar otra partida o consultar sus estadísticas.

## Base de datos

El proyecto utiliza dos tablas.

### Tabla `palabras`

Guarda las palabras disponibles y sus pistas.

| Campo | Tipo | Descripción |
|---|---|---|
| `id` | INTEGER | Identificador único |
| `texto` | TEXT | Palabra que debe adivinarse |
| `pista` | TEXT | Ayuda mostrada al jugador |

Su estructura SQL es:

```sql
CREATE TABLE IF NOT EXISTS palabras
(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    texto TEXT NOT NULL,
    pista TEXT NOT NULL
);
```

### Tabla `partidas`

Guarda el historial de partidas.

| Campo | Tipo | Descripción |
|---|---|---|
| `id` | INTEGER | Identificador único |
| `jugador` | TEXT | Nombre del jugador |
| `palabra` | TEXT | Palabra utilizada |
| `ganada` | INTEGER | `1` si gana y `0` si pierde |
| `errores` | INTEGER | Número de fallos |
| `fecha` | TEXT | Fecha y hora de la partida |

Su estructura SQL es:

```sql
CREATE TABLE IF NOT EXISTS partidas
(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    jugador TEXT NOT NULL,
    palabra TEXT NOT NULL,
    ganada INTEGER NOT NULL,
    errores INTEGER NOT NULL,
    fecha TEXT NOT NULL
);
```

## Consultas SQL utilizadas

### Crear tablas

```sql
CREATE TABLE IF NOT EXISTS ...
```

`IF NOT EXISTS` evita errores cuando la tabla ya ha sido creada.

### Insertar datos

```sql
INSERT INTO palabras (texto, pista)
VALUES (@texto, @pista);
```

Los nombres que comienzan por `@` son parámetros. Sus valores se proporcionan desde C# mediante `AddWithValue`.

### Contar registros

```sql
SELECT COUNT(*) FROM palabras;
```

Se utiliza para comprobar si la tabla está vacía antes de insertar las palabras iniciales.

### Obtener una palabra aleatoria

```sql
SELECT texto, pista
FROM palabras
ORDER BY RANDOM()
LIMIT 1;
```

`ORDER BY RANDOM()` desordena las filas y `LIMIT 1` devuelve solamente una.

### Obtener estadísticas

```sql
SELECT COUNT(*), COALESCE(SUM(ganada), 0)
FROM partidas
WHERE jugador = @jugador;
```

- `COUNT(*)` calcula las partidas totales.
- `SUM(ganada)` calcula las victorias, porque una victoria se guarda como `1`.
- `COALESCE` sustituye un posible valor nulo por `0`.

## Métodos principales de SQLite

El proyecto permite practicar los tres métodos más habituales de `Microsoft.Data.Sqlite`.

### `ExecuteNonQuery()`

Se utiliza con instrucciones que modifican la base de datos pero no devuelven filas:

```csharp
comando.ExecuteNonQuery();
```

Ejemplos:

- `CREATE TABLE`
- `INSERT`
- `UPDATE`
- `DELETE`

### `ExecuteScalar()`

Se utiliza cuando la consulta devuelve un único valor:

```csharp
int cantidad = Convert.ToInt32(comando.ExecuteScalar());
```

En este proyecto se emplea con:

```sql
SELECT COUNT(*) FROM palabras;
```

### `ExecuteReader()`

Se utiliza cuando un `SELECT` devuelve una o varias filas:

```csharp
using (SqliteDataReader lector = comando.ExecuteReader())
{
    while (lector.Read())
    {
        // Leer los campos de la fila actual.
    }
}
```

En este proyecto se emplea para leer palabras y estadísticas.

## Consultas parametrizadas

El proyecto no introduce las variables de C# directamente dentro de las consultas SQL.

En lugar de esto:

```csharp
string sql = $"INSERT INTO partidas (jugador) VALUES ('{jugador}')";
```

se utilizan parámetros:

```csharp
string sql = "INSERT INTO partidas (jugador) VALUES (@jugador)";

using (SqliteCommand comando = new SqliteCommand(sql, conexion))
{
    comando.Parameters.AddWithValue("@jugador", jugador);
    comando.ExecuteNonQuery();
}
```

Esto mantiene separados:

- El código SQL.
- Los datos proporcionados por el usuario.

Además, evita errores con caracteres especiales y reduce el riesgo de inyección SQL.

## Conceptos de C# que se practican

Este proyecto permite trabajar los siguientes contenidos:

### Fundamentos

- Variables.
- Constantes.
- Tipos de datos.
- Operadores.
- Condicionales `if` y `else`.
- Bucles `while`, `do-while` y `foreach`.
- Métodos.
- Parámetros.
- Valores de retorno.

### Colecciones y cadenas

- `List<char>`.
- Arrays.
- `string`.
- `Contains`.
- `Trim`.
- `ToLower`.
- `ToUpper`.
- `string.Join`.
- Interpolación con `$"..."`.

### Programación orientada a objetos

- Clases.
- Objetos.
- Constructores.
- Propiedades.
- Encapsulación.
- Métodos públicos y privados.
- Separación de responsabilidades.
- Composición entre objetos.

### Gestión de recursos

Los bloques `using` garantizan que las conexiones, comandos y lectores se cierren correctamente:

```csharp
using (SqliteConnection conexion = new SqliteConnection(CadenaConexion))
{
    conexion.Open();

    // Operaciones con la base de datos.
}
```

## Qué se aprende con este proyecto

Al completar y estudiar este juego se aprende a:

- Crear un proyecto de consola con .NET.
- Dividir un programa en varios ficheros.
- Diseñar clases con responsabilidades claras.
- Comunicar objetos entre sí.
- Crear una base de datos SQLite desde C#.
- Diseñar tablas sencillas.
- Ejecutar instrucciones SQL.
- Insertar y consultar datos.
- Utilizar parámetros en consultas SQL.
- Leer registros con `SqliteDataReader`.
- Guardar información de forma permanente.
- Implementar la lógica de un juego por turnos.
- Validar datos introducidos por teclado.
- Mantener separadas la lógica del juego y la persistencia.

## Añadir nuevas palabras

Las palabras iniciales se encuentran en `BaseDatos.cs`:

```csharp
Palabra[] palabras =
{
    new Palabra("ordenador", "Máquina utilizada para ejecutar programas"),
    new Palabra("variable", "Espacio en el que un programa guarda un dato"),
    new Palabra("consola", "Ventana de texto en la que aparece este juego")
};
```

Para añadir una nueva palabra:

```csharp
new Palabra("clase", "Plantilla utilizada para crear objetos")
```

Las palabras iniciales solo se insertan cuando la tabla está vacía.

Si el fichero `ahorcado.db` ya existe, las nuevas palabras del array no se añadirán automáticamente. Para volver a crear la base de datos inicial:

1. Cierra el programa.
2. Borra el fichero `ahorcado.db`.
3. Ejecuta nuevamente el proyecto.

> Al borrar `ahorcado.db` también se elimina el historial de partidas.

## Posibles mejoras

Algunas ampliaciones sencillas para continuar practicando son:

- Añadir categorías.
- Elegir la dificultad.
- Limitar las palabras según su longitud.
- Evitar que una palabra se repita en partidas consecutivas.
- Permitir adivinar la palabra completa.
- Crear un sistema de puntuación.
- Mostrar una clasificación.
- Guardar el tiempo empleado.
- Añadir pistas que resten puntos.
- Crear un menú para insertar nuevas palabras.
- Permitir borrar o modificar palabras.
- Mostrar el historial de partidas.
- Añadir diferentes dibujos del ahorcado.
- Usar palabras con tildes normalizando la entrada.

## Objetivo didáctico

El objetivo principal no es crear la versión más eficiente posible, sino presentar un ejemplo:

- Fácil de leer.
- Fácil de modificar.
- Dividido en partes comprensibles.
- Adecuado para principiantes.
- Útil para introducir SQLite sin ocultar los pasos importantes.

Por ese motivo, algunas operaciones abren su propia conexión y repiten parte de la estructura. Esta repetición permite observar con claridad el proceso completo:

```text
Crear conexión
      ↓
Abrir conexión
      ↓
Escribir consulta SQL
      ↓
Crear comando
      ↓
Añadir parámetros
      ↓
Ejecutar consulta
      ↓
Cerrar recursos automáticamente
```

## Autor

Proyecto educativo creado para practicar programación en C#, programación orientada a objetos y acceso a bases de datos SQLite.

## Licencia

Este proyecto puede utilizarse y modificarse con fines educativos.

Puedes añadir un fichero `LICENSE` con la licencia que prefieras, por ejemplo la licencia MIT.
