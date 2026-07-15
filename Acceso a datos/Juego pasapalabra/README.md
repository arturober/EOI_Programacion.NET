# Pasapalabra con C# y SQLite

Aplicación de consola educativa inspirada en el juego **Pasapalabra**, creada con **C#**, **.NET 8** y una base de datos **SQLite**.

El proyecto conserva el estilo del ejemplo del ahorcado: código sencillo, pocas clases, consultas SQL visibles, mapeo directo entre tablas y objetos, menús con `while` y `switch`, y consultas parametrizadas con `AddWithValue`.

La base de datos incluida contiene **5 temas** y **135 preguntas en español de España**. Cada tema tiene exactamente **27 preguntas**, una para cada letra del alfabeto español, incluida la `Ñ`.

## Características principales

- Rosco completo de 27 letras: `A-Z` y `Ñ`.
- Selección de un tema concreto o mezcla de todos los temas.
- Una pregunta aleatoria para cada letra.
- El programa decide automáticamente si debe mostrar:
  - `Empieza por la A...`
  - `Contiene la X...`
- Comando `PASAPALABRA` para dejar una letra pendiente.
- Las preguntas pendientes vuelven a aparecer en vueltas posteriores.
- Comando `SALIR` para abandonar la partida.
- Corrección de respuestas ignorando mayúsculas, tildes y espacios repetidos.
- La `ñ` sigue siendo diferente de la `n`.
- Resultado final con aciertos, fallos y preguntas pendientes.
- CRUD completo de preguntas.
- CRUD completo de temas.
- Protección para impedir borrar un tema que todavía contiene preguntas.
- Consultas SQL parametrizadas para evitar SQL Injection.
- Mapeo de la tabla `temas` a la clase `Tema`.
- Mapeo de la tabla `preguntas` a la clase `Pregunta`.

## Contenido de la base de datos

| Tema | Preguntas | Letras disponibles |
|---|---:|---:|
| Cultura general | 27 | 27 |
| Ciencia y naturaleza | 27 | 27 |
| Geografía | 27 | 27 |
| Informática | 27 | 27 |
| Lengua y literatura | 27 | 27 |
| **Total** | **135** | **5 roscos completos** |

Para las letras menos habituales, como `K`, `W`, `X` o `Ñ`, algunas respuestas contienen la letra en lugar de comenzar por ella.

## Tecnologías utilizadas

- C#.
- .NET 8.
- SQLite.
- `Microsoft.Data.Sqlite`.
- ADO.NET sin ORM.

La dependencia utilizada es:

```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.12" />
```

## Estructura del proyecto

```text
PasapalabraConSQLiteMapeado/
│
├── Program.cs
├── BaseDatos.cs
├── Tema.cs
├── Pregunta.cs
├── TextoUtil.cs
├── GestorPreguntas.cs
├── GestorTemas.cs
├── JuegoPasapalabra.cs
├── PasapalabraConSQLite.csproj
├── pasapalabra.db
└── README.md
```

### `Program.cs`

Es el punto de entrada de la aplicación.

- Abre la conexión con SQLite.
- Crea las tablas si no existen.
- Crea los gestores y el objeto del juego.
- Muestra el menú principal.

### `BaseDatos.cs`

Centraliza:

- La cadena de conexión.
- La creación de la conexión.
- La activación de claves externas.
- La creación de las tablas y del índice.

### `Tema.cs`

Representa una fila de la tabla `temas`.

Incluye:

- Propiedades `Id`, `Nombre` y `Descripcion`.
- Constructores.
- `ToString()`.
- `Insertar()`.
- `Actualizar()`.
- `Borrar()`.
- `Listar()`.
- `Buscar()`.
- `BuscarPorId()`.
- `Existe()`.

### `Pregunta.cs`

Representa una fila de la tabla `preguntas`.

Incluye:

- Propiedades `Id`, `Letra`, `Respuesta`, `Definicion` y `Tema`.
- CRUD completo.
- Búsqueda por texto e ID.
- Comprobación de respuestas duplicadas dentro de un tema.
- Recuento de preguntas y letras.
- Creación aleatoria de un rosco.
- Generación automática del enunciado `Empieza por...` o `Contiene la...`.

### `GestorPreguntas.cs`

Muestra el menú del CRUD de preguntas y solicita los datos al usuario.

Antes de modificar o eliminar, muestra el listado completo para que se vean los identificadores disponibles.

### `GestorTemas.cs`

Muestra el menú del CRUD de temas.

Al eliminar un tema, comprueba primero si contiene preguntas. Si tiene preguntas asociadas, no permite borrarlo porque la clave externa utiliza `ON DELETE RESTRICT`.

### `JuegoPasapalabra.cs`

Contiene la lógica del juego:

- Selección de tema.
- Obtención de las 27 preguntas.
- Control del estado de cada letra.
- Gestión de `PASAPALABRA`.
- Comprobación de respuestas.
- Repetición de las preguntas pendientes.
- Presentación del resultado final.

### `TextoUtil.cs`

Contiene métodos auxiliares para:

- Normalizar textos.
- Ignorar tildes y mayúsculas al comparar.
- Validar respuestas.
- Validar letras del rosco.
- Leer textos, números y confirmaciones.
- Pausar la consola.

## Base de datos

### Tabla `temas`

```sql
CREATE TABLE temas (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL UNIQUE COLLATE NOCASE,
    descripcion TEXT NOT NULL
);
```

### Tabla `preguntas`

```sql
CREATE TABLE preguntas (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    letra TEXT NOT NULL,
    respuesta TEXT NOT NULL,
    respuesta_normalizada TEXT NOT NULL,
    definicion TEXT NOT NULL,
    tema_id INTEGER NOT NULL,
    UNIQUE (respuesta_normalizada, tema_id),
    FOREIGN KEY (tema_id)
        REFERENCES temas(id)
        ON DELETE RESTRICT
);
```

La relación es:

```text
Un tema contiene muchas preguntas.
Cada pregunta pertenece a un solo tema.
```

En C#, una pregunta contiene directamente un objeto `Tema`:

```csharp
public Tema Tema
{
    get { return tema; }
    set { tema = value; }
}
```

En SQLite se guarda únicamente `tema_id`.

## Relación entre tablas mediante WHERE

Las consultas mantienen el mismo enfoque didáctico del proyecto de referencia:

```sql
SELECT
    p.id AS pregunta_id,
    p.letra,
    p.respuesta,
    p.definicion,
    t.id AS tema_id,
    t.nombre,
    t.descripcion
FROM preguntas p, temas t
WHERE p.tema_id = t.id;
```

La condición:

```sql
WHERE p.tema_id = t.id
```

relaciona cada pregunta con su tema.

## CRUD de preguntas

Menú disponible:

```text
GESTIÓN DE PREGUNTAS
=====================
1. Añadir pregunta
2. Mostrar todas las preguntas
3. Buscar pregunta
4. Modificar pregunta
5. Eliminar pregunta
0. Volver al menú principal
```

### Crear

Se solicitan:

1. Letra del rosco.
2. Respuesta.
3. Definición.
4. Tema.

La respuesta debe contener la letra seleccionada. De esta forma el enunciado siempre será coherente.

### Consultar

Las preguntas se muestran con:

- ID.
- Letra.
- Respuesta.
- Definición.
- Tema.

### Buscar

La búsqueda localiza coincidencias tanto en la respuesta como en la definición.

### Modificar

Permite cambiar:

- Letra.
- Respuesta.
- Definición.
- Tema.

Pulsar `Enter` conserva el valor actual.

### Eliminar

Muestra la pregunta y solicita confirmación antes de ejecutar el `DELETE`.

## CRUD de temas

Menú disponible:

```text
GESTIÓN DE TEMAS
=================
1. Añadir tema
2. Mostrar todos los temas
3. Buscar tema
4. Modificar tema
5. Eliminar tema
0. Volver al menú principal
```

Cada tema muestra cuántas preguntas y letras diferentes contiene.

Un tema necesita disponer de las 27 letras para poder seleccionarse en el juego. Se pueden crear temas incompletos y añadir las preguntas poco a poco, pero el programa no permitirá jugar con ellos hasta completar el rosco.

## Cómo jugar

Al comenzar aparece el menú principal:

```text
====================================
         JUEGO DE PASAPALABRA
====================================
1. Jugar
2. Gestionar preguntas (CRUD)
3. Gestionar temas (CRUD)
4. Mostrar temas disponibles
0. Salir
```

Después se selecciona un tema:

```text
ELIGE UN TEMA
=============
0. Todos los temas (27 letras disponibles)
1. Cultura general (27 preguntas, 27 letras)
2. Ciencia y naturaleza (27 preguntas, 27 letras)
...
```

Durante la partida el rosco utiliza estos símbolos:

```text
[A?]  Pregunta pendiente
[A+]  Respuesta correcta
[A-]  Respuesta incorrecta
```

Ejemplo de pregunta:

```text
Empieza por la A: Conjunto ordenado de pasos que permite resolver un problema.
Respuesta, PASAPALABRA o SALIR:
```

Si se escribe `PASAPALABRA`, la letra continúa pendiente y volverá a aparecer más adelante.

Una respuesta incorrecta se marca como fallo y se muestra la solución correcta.

## Comparación de respuestas

El programa normaliza las respuestas antes de compararlas.

Por ejemplo, se consideran equivalentes:

```text
JÚPITER
jupiter
Júpiter
```

También se eliminan los espacios repetidos.

La `ñ` se conserva como una letra distinta, por lo que:

```text
ano
```

no es igual que:

```text
año
```

## Consultas parametrizadas

Los textos escritos por el usuario nunca se concatenan directamente dentro del SQL.

Ejemplo:

```csharp
string sql =
    "SELECT COUNT(*) " +
    "FROM preguntas " +
    "WHERE respuesta_normalizada = @normalizada " +
    "AND tema_id = @temaId " +
    "AND id <> @idIgnorado";

SqliteCommand cmd = new SqliteCommand(sql, conexion);

cmd.Parameters.AddWithValue(
    "@normalizada",
    TextoUtil.NormalizarParaComparar(respuesta));
cmd.Parameters.AddWithValue("@temaId", temaId);
cmd.Parameters.AddWithValue("@idIgnorado", idIgnorado);
```

Esto permite enseñar:

- Prevención de SQL Injection.
- Separación entre la orden SQL y los datos.
- Uso de `AddWithValue`.
- Manejo correcto de comillas y caracteres especiales.

## Instalación y ejecución

### Desde Visual Studio

1. Abre `PasapalabraConSQLite.csproj`.
2. Espera a que se restaure el paquete NuGet.
3. Comprueba que `pasapalabra.db` está dentro del proyecto.
4. Ejecuta con `Ctrl + F5`.

### Desde una terminal

```bash
dotnet restore
dotnet run
```

Para compilar:

```bash
dotnet build
```

El fichero del proyecto copia la base de datos a la carpeta de salida:

```xml
<None Update="pasapalabra.db">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

## Objetivos didácticos

Este proyecto permite practicar:

- Clases y objetos.
- Atributos privados y propiedades públicas.
- Constructores.
- Composición de objetos.
- Métodos estáticos y de instancia.
- Sobrescritura de `ToString()`.
- Listas y diccionarios genéricos.
- Bucles y condicionales.
- Menús con `switch`.
- Valores anulables como `Tema?` y `Pregunta?`.
- Conexión con SQLite.
- `INSERT`, `SELECT`, `UPDATE` y `DELETE`.
- `ExecuteNonQuery()`, `ExecuteReader()` y `ExecuteScalar()`.
- Claves primarias y externas.
- Restricciones `UNIQUE` y `NOT NULL`.
- Consultas parametrizadas.
- Mapeo de filas de una tabla a objetos de C#.
- Validación y normalización de texto.
- Separación de responsabilidades.

## Posibles ampliaciones

- Añadir un límite de tiempo.
- Guardar jugadores y partidas.
- Crear un ranking.
- Añadir niveles de dificultad.
- Restar puntos por fallos.
- Permitir dos jugadores.
- Añadir colores a las letras del rosco.
- Evitar que se repitan preguntas entre partidas.
- Exportar preguntas a CSV.
- Importar preguntas desde un fichero.
- Guardar estadísticas por tema.
- Añadir pruebas unitarias.

## Solución de problemas

### El listado está vacío

Comprueba que el programa está utilizando la copia correcta de `pasapalabra.db`. La ruta de conexión es relativa:

```csharp
private const string CadenaConexion =
    "Data Source=pasapalabra.db";
```

La base de datos utilizada normalmente estará en la carpeta de ejecución, por ejemplo:

```text
bin/Debug/net8.0/pasapalabra.db
```

### Un tema no aparece como jugable

Debe contener al menos una pregunta para cada una de las 27 letras. El menú indica cuántas letras diferentes tiene disponibles.

### No se puede eliminar un tema

Es el comportamiento esperado cuando tiene preguntas asociadas. Primero hay que eliminar esas preguntas o asignarlas a otro tema.

### No se encuentra `Microsoft.Data.Sqlite`

Ejecuta:

```bash
dotnet restore
```

O añade el paquete manualmente:

```bash
dotnet add package Microsoft.Data.Sqlite --version 8.0.12
```

---

Proyecto creado con finalidad educativa, priorizando la legibilidad y la facilidad de modificación frente a arquitecturas más complejas.
