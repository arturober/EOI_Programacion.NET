# Agenda telefónica con C# y SQLite

Aplicación de consola sencilla que permite gestionar una agenda telefónica utilizando **C#**, **.NET** y una base de datos **SQLite**.

El programa permite guardar personas junto con sus números de teléfono y realizar las operaciones básicas de cualquier aplicación de gestión de datos:

* Añadir personas.
* Mostrar todas las personas.
* Buscar personas.
* Modificar personas.
* Eliminar personas.

Este proyecto está especialmente pensado para iniciarse en el uso de bases de datos relacionales.

---

## Objetivo del proyecto

El objetivo principal es aprender a conectar una aplicación de consola escrita en C# con una base de datos SQLite.

A través de este proyecto se implementa un CRUD completo.

CRUD es un acrónimo formado por las cuatro operaciones fundamentales que se realizan sobre los datos:

| Operación | Significado | Acción en la agenda     |
| --------- | ----------- | ----------------------- |
| **C**     | Create      | Crear una nueva persona |
| **R**     | Read        | Leer y mostrar personas |
| **U**     | Update      | Modificar una persona   |
| **D**     | Delete      | Eliminar una persona    |

Además, el programa incorpora una búsqueda por nombre o teléfono.

---

## Funcionalidades

La aplicación muestra un menú como el siguiente:

```text
=================================
         AGENDA TELEFÓNICA
=================================
1. Añadir persona
2. Mostrar todas las personas
3. Modificar una persona
4. Eliminar una persona
5. Buscar por nombre o teléfono
0. Salir
=================================
```

### Añadir una persona

Permite introducir el nombre y el teléfono de una nueva persona.

```text
Nombre: Ana Pérez
Teléfono: 612 345 678
```

Los datos se guardan permanentemente en la base de datos.

### Mostrar todas las personas

Muestra todos los contactos almacenados, ordenados alfabéticamente por nombre.

```text
[1] Ana Pérez - 612 345 678
[2] Carlos López - 965 123 456
[3] María García - 644 987 321
```

El número situado entre corchetes es el identificador único de cada persona.

### Modificar una persona

Permite cambiar el nombre y el teléfono de una persona existente.

Para identificar a la persona que se quiere modificar, se utiliza su `id`.

### Eliminar una persona

Permite borrar una persona de la agenda utilizando su `id`.

Antes de eliminarla, se solicita confirmación para evitar borrados accidentales.

### Buscar personas

Permite buscar coincidencias parciales tanto en el nombre como en el teléfono.

Por ejemplo, una búsqueda de:

```text
Ana
```

podría encontrar:

```text
Ana
Ana Pérez
Mariana López
```

También es posible buscar una parte de un teléfono:

```text
612
```

---

## Tecnologías utilizadas

* **C#**
* **.NET**
* **SQLite**
* **Microsoft.Data.Sqlite**
* Aplicación de consola

---

## Requisitos

Para ejecutar el proyecto es necesario tener instalado el SDK de .NET.

Puedes comprobar si está instalado ejecutando:

```bash
dotnet --version
```

Si aparece un número de versión, significa que .NET está instalado correctamente.

---

## Creación del proyecto

Para crear un proyecto equivalente desde cero, se puede utilizar el siguiente comando:

```bash
dotnet new console -n AgendaTelefonica
```

Después, entra en la carpeta del proyecto:

```bash
cd AgendaTelefonica
```

La aplicación necesita el paquete `Microsoft.Data.Sqlite` para comunicarse con SQLite.

Se instala con este comando:

```bash
dotnet add package Microsoft.Data.Sqlite
```

Por último, sustituye el contenido del archivo `Program.cs` por el código de la aplicación.

---

## Ejecución

Para ejecutar el programa:

```bash
dotnet run
```

La primera vez que se ejecute se creará automáticamente un archivo llamado:

```text
agenda.db
```

Este archivo contiene la base de datos SQLite.

Los datos permanecerán almacenados aunque se cierre la aplicación.

---

## Estructura del proyecto

La estructura básica del proyecto es la siguiente:

```text
AgendaTelefonica/
├── AgendaTelefonica.csproj
├── Program.cs
├── agenda.db
└── README.md
```

### `AgendaTelefonica.csproj`

Contiene la configuración del proyecto y la referencia al paquete `Microsoft.Data.Sqlite`.

### `Program.cs`

Contiene el código completo de la aplicación:

* Creación de la conexión.
* Creación de la tabla.
* Menú principal.
* Inserción de personas.
* Consulta de personas.
* Modificación de personas.
* Eliminación de personas.
* Búsqueda de contactos.
* Validación de los datos introducidos.

### `agenda.db`

Es el archivo de la base de datos SQLite.

Se crea automáticamente al ejecutar el programa por primera vez.

No es necesario crear previamente la base de datos ni instalar un servidor.

---

# Implementación de la base de datos

## Cadena de conexión

La conexión con SQLite se configura mediante una cadena de conexión:

```csharp
string cadenaConexion = "Data Source=agenda.db";
```

`Data Source` indica el archivo que contiene la base de datos.

Si el archivo no existe, SQLite lo crea automáticamente.

---

## Conexión con SQLite

La conexión se crea utilizando la clase `SqliteConnection`:

```csharp
SqliteConnection conexion =
    new SqliteConnection(cadenaConexion);
```

Antes de ejecutar instrucciones SQL, es necesario abrir la conexión:

```csharp
conexion.Open();
```

La conexión se utiliza dentro de un bloque `using`:

```csharp
using (conexion)
{
    conexion.Open();

    // Operaciones con la base de datos
}
```

El bloque `using` garantiza que la conexión se cierre y libere correctamente cuando deje de utilizarse, incluso si se produce algún error durante la ejecución.

---

## Tabla `personas`

La aplicación utiliza una única tabla llamada `personas`.

Su estructura es la siguiente:

| Columna    | Tipo    | Descripción          |
| ---------- | ------- | -------------------- |
| `id`       | INTEGER | Identificador único  |
| `nombre`   | TEXT    | Nombre de la persona |
| `telefono` | TEXT    | Número de teléfono   |

La tabla se crea con esta instrucción SQL:

```sql
CREATE TABLE IF NOT EXISTS personas (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    telefono TEXT NOT NULL
);
```

### Identificador

La columna `id` es la clave principal:

```sql
id INTEGER PRIMARY KEY AUTOINCREMENT
```

SQLite genera automáticamente un identificador diferente para cada persona:

```text
1
2
3
4
```

El usuario no tiene que introducir este valor.

### Nombre

El nombre se almacena como texto:

```sql
nombre TEXT NOT NULL
```

`NOT NULL` indica que este campo es obligatorio.

### Teléfono

El teléfono también se almacena como texto:

```sql
telefono TEXT NOT NULL
```

Aunque un teléfono contiene números, no se guarda como `INTEGER`.

Un número de teléfono puede contener:

* Espacios.
* El símbolo `+`.
* Guiones.
* Paréntesis.
* Ceros iniciales.

Por ejemplo:

```text
+34 612 345 678
965-123-456
0034 600 123 123
```

Los teléfonos no se utilizan normalmente para realizar operaciones matemáticas, por lo que resulta más apropiado guardarlos como texto.

---

# Operaciones CRUD

## Crear una persona

Para añadir una persona se utiliza la instrucción SQL `INSERT`:

```sql
INSERT INTO personas (nombre, telefono)
VALUES (@nombre, @telefono);
```

Los valores se pasan mediante parámetros:

```csharp
comando.Parameters.AddWithValue(
    "@nombre", nombre);

comando.Parameters.AddWithValue(
    "@telefono", telefono);
```

Después, la instrucción se ejecuta mediante:

```csharp
int filas = comando.ExecuteNonQuery();
```

`ExecuteNonQuery()` se utiliza para ejecutar instrucciones que modifican la base de datos, pero no devuelven una lista de resultados.

Se utiliza habitualmente con:

* `CREATE`
* `INSERT`
* `UPDATE`
* `DELETE`

El método devuelve el número de filas afectadas.

Si se inserta correctamente una persona, normalmente devuelve `1`.

---

## Leer las personas

Para consultar las personas se utiliza una instrucción `SELECT`:

```sql
SELECT id, nombre, telefono
FROM personas
ORDER BY nombre;
```

`ORDER BY nombre` hace que los resultados se muestren ordenados alfabéticamente.

La consulta se ejecuta con:

```csharp
SqliteDataReader lector =
    comando.ExecuteReader();
```

El objeto `SqliteDataReader` permite recorrer las filas obtenidas una a una.

```csharp
while (lector.Read())
{
    int id = lector.GetInt32(0);
    string nombre = lector.GetString(1);
    string telefono = lector.GetString(2);
}
```

Las posiciones de las columnas comienzan en cero:

| Posición | Columna    |
| -------: | ---------- |
|        0 | `id`       |
|        1 | `nombre`   |
|        2 | `telefono` |

---

## Modificar una persona

Para modificar una persona se utiliza la instrucción `UPDATE`:

```sql
UPDATE personas
SET nombre = @nombre,
    telefono = @telefono
WHERE id = @id;
```

La cláusula `WHERE` indica qué persona debe modificarse.

Es muy importante incluirla. Sin `WHERE`, se modificarían todas las filas de la tabla.

Por ejemplo, esta instrucción sería peligrosa:

```sql
UPDATE personas
SET nombre = 'Ana';
```

Cambiaría el nombre de todas las personas guardadas.

En cambio:

```sql
UPDATE personas
SET nombre = 'Ana'
WHERE id = 3;
```

solamente modifica la persona cuyo identificador es `3`.

---

## Eliminar una persona

Para eliminar una persona se utiliza `DELETE`:

```sql
DELETE FROM personas
WHERE id = @id;
```

De nuevo, la cláusula `WHERE` es fundamental.

Sin ella:

```sql
DELETE FROM personas;
```

se eliminarían todas las personas de la agenda.

Por seguridad, el programa pide confirmación antes de realizar la eliminación.

---

## Buscar personas

La búsqueda utiliza el operador `LIKE`:

```sql
SELECT id, nombre, telefono
FROM personas
WHERE nombre LIKE @texto
   OR telefono LIKE @texto
ORDER BY nombre;
```

El parámetro se construye añadiendo el símbolo `%` antes y después del texto:

```csharp
comando.Parameters.AddWithValue(
    "@texto", "%" + texto + "%");
```

En SQL, el símbolo `%` representa cualquier cantidad de caracteres.

Por ejemplo:

```text
%Ana%
```

permite encontrar cualquier valor que contenga el texto `Ana`.

---

# Uso de parámetros SQL

En el proyecto, los valores introducidos por el usuario no se concatenan directamente dentro de las instrucciones SQL.

No se hace esto:

```csharp
string sql =
    "INSERT INTO personas VALUES ('" +
    nombre + "', '" + telefono + "')";
```

En su lugar, se utilizan parámetros:

```csharp
string sql =
    "INSERT INTO personas (nombre, telefono) " +
    "VALUES (@nombre, @telefono)";
```

Y después se asignan los valores:

```csharp
comando.Parameters.AddWithValue(
    "@nombre", nombre);
```

Los parámetros ofrecen varias ventajas:

* Separan la instrucción SQL de los datos.
* Evitan problemas con comillas y caracteres especiales.
* Hacen que el código sea más fácil de leer.
* Reducen el riesgo de inyección SQL.
* Permiten reutilizar una misma instrucción con valores diferentes.

---

# Métodos del programa

El código se divide en varios métodos para que cada uno tenga una responsabilidad concreta.

| Método                   | Responsabilidad                         |
| ------------------------ | --------------------------------------- |
| `Main()`                 | Iniciar el programa y controlar el menú |
| `MostrarMenu()`          | Mostrar las opciones disponibles        |
| `CrearTabla()`           | Crear la tabla si no existe             |
| `CrearPersona()`         | Añadir una persona                      |
| `MostrarPersonas()`      | Mostrar todos los contactos             |
| `ModificarPersona()`     | Modificar una persona                   |
| `EliminarPersona()`      | Eliminar una persona                    |
| `BuscarPersonas()`       | Buscar por nombre o teléfono            |
| `LeerTextoObligatorio()` | Validar que un texto no esté vacío      |
| `LeerNumeroEntero()`     | Validar la introducción de un número    |
| `Pausar()`               | Detener temporalmente el programa       |

Esta separación mejora la legibilidad y permite localizar más fácilmente cada parte del código.

---

# Validación de datos

La aplicación incluye validaciones sencillas para evitar algunos errores habituales.

## Textos obligatorios

El método `LeerTextoObligatorio()` impide que el usuario introduzca un nombre o un teléfono vacío.

```csharp
if (texto != "")
{
    return texto;
}
```

El programa seguirá preguntando hasta que el usuario introduzca un valor.

## Números enteros

Para leer los identificadores se utiliza `int.TryParse()`:

```csharp
bool esNumero =
    int.TryParse(texto, out int numero);
```

A diferencia de `int.Parse()`, `TryParse()` no provoca una excepción si el usuario escribe un valor incorrecto.

Por ejemplo:

```text
abc
```

En ese caso devuelve `false` y el programa puede volver a solicitar el dato.

También se comprueba que el identificador sea mayor que cero.

---

# Conceptos de C# que se practican

Este proyecto permite practicar los siguientes conceptos:

## Variables y tipos de datos

Se utilizan tipos como:

```csharp
string
int
bool
```

## Condicionales

El programa utiliza estructuras `if` para comprobar diferentes situaciones:

```csharp
if (filas == 1)
{
    Console.WriteLine(
        "La persona se ha añadido correctamente.");
}
```

## Bucles

El menú principal utiliza un bucle `while`:

```csharp
while (!salir)
{
    // Mostrar y procesar el menú
}
```

También se utilizan bucles para validar datos y recorrer los resultados de la base de datos.

## `switch`

Las opciones del menú se procesan mediante un `switch`:

```csharp
switch (opcion)
{
    case "1":
        CrearPersona(conexion);
        break;

    case "2":
        MostrarPersonas(conexion);
        break;
}
```

## Métodos

El programa se divide en métodos para organizar mejor el código y evitar que todo esté incluido dentro de `Main()`.

## Parámetros y valores de retorno

Algunos métodos reciben la conexión como parámetro:

```csharp
static void CrearPersona(
    SqliteConnection conexion)
```

Otros devuelven un resultado:

```csharp
static bool MostrarPersonas(
    SqliteConnection conexion)
```

## Interpolación de cadenas

Los datos se muestran utilizando interpolación:

```csharp
Console.WriteLine(
    $"[{id}] {nombre} - {telefono}");
```

## Operador de fusión de null

Se utiliza el operador `??`:

```csharp
string texto =
    Console.ReadLine() ?? "";
```

Este operador permite utilizar una cadena vacía si `Console.ReadLine()` devuelve `null`.

## Bloques `using`

Se utilizan para liberar correctamente recursos como:

* Conexiones.
* Comandos.
* Lectores de datos.

---

# Conceptos de SQLite que se practican

El proyecto introduce los conceptos fundamentales de una base de datos relacional:

* Bases de datos.
* Tablas.
* Filas y columnas.
* Claves principales.
* Identificadores autoincrementales.
* Tipos de datos.
* Campos obligatorios.
* Consultas SQL.
* Parámetros SQL.
* Inserciones.
* Modificaciones.
* Eliminaciones.
* Búsquedas.
* Ordenación de resultados.

También permite practicar las principales instrucciones SQL:

```sql
CREATE TABLE
INSERT INTO
SELECT
UPDATE
DELETE
WHERE
ORDER BY
LIKE
```

---

# ¿Por qué utilizar SQLite?

SQLite es una base de datos especialmente adecuada para proyectos pequeños y educativos.

Sus principales ventajas son:

* No necesita instalar un servidor.
* La base de datos se guarda en un único archivo.
* Es fácil de utilizar desde C#.
* Permite aprender SQL real.
* Los datos permanecen almacenados al cerrar el programa.
* Es adecuada para aplicaciones de escritorio, móviles y pequeños proyectos.

A diferencia de otros sistemas como SQL Server, MySQL o PostgreSQL, SQLite no necesita mantener un servicio de base de datos ejecutándose.

---

# Posibles mejoras

Una vez comprendida la versión básica, se pueden añadir nuevas funcionalidades.

## Añadir más datos

Se podrían incorporar nuevos campos:

* Apellidos.
* Correo electrónico.
* Dirección.
* Fecha de nacimiento.
* Empresa.
* Notas.
* Categoría del contacto.

## Permitir varios teléfonos por persona

En la versión actual, cada persona tiene un único teléfono.

Como mejora, se podrían crear dos tablas:

```text
personas
telefonos
```

Una persona podría estar relacionada con varios teléfonos.

Esto permitiría aprender relaciones uno a muchos entre tablas.

## Evitar teléfonos duplicados

Se podría comprobar si un teléfono ya existe antes de guardarlo.

También se podría utilizar una restricción `UNIQUE`:

```sql
telefono TEXT NOT NULL UNIQUE
```

## Modificar solo algunos datos

La versión actual solicita de nuevo el nombre y el teléfono.

Se podría permitir dejar un campo vacío para conservar su valor anterior.

## Añadir paginación

Si la agenda tuviera muchos contactos, se podrían mostrar por páginas.

## Añadir categorías

Por ejemplo:

* Familia.
* Amigos.
* Trabajo.
* Estudios.
* Otros.

## Exportar datos

Se podrían exportar los contactos a:

* CSV.
* JSON.
* Texto.
* XML.

## Importar contactos

También se podrían leer contactos desde un archivo externo.

## Separar el programa en clases

Cuando se conozca mejor la programación orientada a objetos, el proyecto se puede dividir en varias clases:

```text
Persona.cs
BaseDatos.cs
Agenda.cs
Program.cs
```

Por ejemplo, una clase `Persona` podría representar cada contacto:

```csharp
class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
}
```

## Crear una interfaz gráfica

La aplicación también podría transformarse en una aplicación con ventanas utilizando:

* Windows Forms.
* WPF.
* .NET MAUI.
* Avalonia.
* Godot con C#.

---

# Problemas frecuentes

## No se reconoce `Microsoft.Data.Sqlite`

Asegúrate de haber instalado el paquete:

```bash
dotnet add package Microsoft.Data.Sqlite
```

Después, restaura las dependencias:

```bash
dotnet restore
```

## No aparece el archivo `agenda.db`

El archivo se crea al ejecutar el programa y abrir la conexión.

Ejecuta:

```bash
dotnet run
```

Normalmente aparecerá en la carpeta desde la que se está ejecutando la aplicación.

## Se repiten los contactos

Cada vez que se utiliza la opción de añadir una persona se crea una nueva fila.

La base de datos no elimina los datos al cerrar el programa.

Los contactos anteriores seguirán almacenados en `agenda.db`.

## Quiero empezar con una base de datos vacía

Cierra el programa y elimina el archivo:

```text
agenda.db
```

Al ejecutar de nuevo la aplicación, SQLite creará una base de datos vacía.

No elimines el archivo si quieres conservar los contactos.

---

# Qué se aprende con este proyecto

Al completar y estudiar esta aplicación se aprende a:

1. Crear un proyecto de consola con .NET.
2. Instalar paquetes mediante NuGet.
3. Conectar C# con una base de datos SQLite.
4. Crear una base de datos automáticamente.
5. Crear tablas mediante SQL.
6. Insertar datos.
7. Consultar y recorrer resultados.
8. Modificar registros.
9. Eliminar registros.
10. Buscar información mediante `LIKE`.
11. Utilizar parámetros SQL.
12. Validar datos introducidos por teclado.
13. Crear menús de consola.
14. Dividir un programa en métodos.
15. Utilizar correctamente conexiones, comandos y lectores.
16. Comprender el funcionamiento básico de un CRUD.
17. Conservar información entre diferentes ejecuciones del programa.

---

# Finalidad educativa

Este proyecto prioriza que el código sea:

* Fácil de leer.
* Fácil de explicar.
* Fácil de modificar.
* Adecuado para principiantes.
* Similar al estilo habitual de una aplicación de consola en C#.

Algunas partes podrían escribirse con menos líneas o utilizando características más avanzadas de C#, pero se ha preferido una implementación explícita y didáctica.

La intención es que cada operación se pueda seguir paso a paso y que el alumnado comprenda qué ocurre entre la aplicación, la instrucción SQL y la base de datos.

---

## Licencia

Este proyecto puede utilizarse y modificarse libremente con fines educativos.
