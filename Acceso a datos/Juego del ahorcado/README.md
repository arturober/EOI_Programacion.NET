# Juego del Ahorcado con C#, SQLite, CRUD y colores

Aplicación educativa de consola realizada con **C#**, **.NET 8** y **SQLite**. Mantiene un diseño sencillo, orientado a alumnado que está aprendiendo programación orientada a objetos, consultas parametrizadas y operaciones CRUD.

## Novedades de esta versión

- CRUD completo de palabras.
- CRUD completo de temas.
- Búsqueda de temas por nombre o descripción.
- Protección frente a nombres de tema duplicados.
- No se permite eliminar un tema mientras tenga palabras asociadas.
- Colores en el menú y, principalmente, en la representación del ahorcado.
- El dibujo aparece verde, amarillo o rojo según el número de errores.
- La palabra oculta, la pista, los avisos de victoria y los de derrota usan colores diferentes.
- Se conserva el mapeo de tablas a objetos:
  - tabla `temas` → clase `Tema`;
  - tabla `palabras` → clase `Palabra`.

## Menú principal

```text
====================================
          JUEGO DEL AHORCADO
====================================
1. Jugar
2. Gestionar palabras (CRUD)
3. Gestionar temas (CRUD)
4. Mostrar temas disponibles
0. Salir
```

## CRUD de temas

El nuevo fichero `GestorTemas.cs` contiene la interfaz de consola para:

1. Añadir un tema.
2. Mostrar todos los temas.
3. Buscar un tema.
4. Modificar un tema.
5. Eliminar un tema.

La clase `Tema` contiene las operaciones de acceso a su tabla:

| Operación | Método | SQL |
|---|---|---|
| Crear | `Tema.Insertar()` | `INSERT` |
| Consultar | `Tema.Listar()` | `SELECT` |
| Buscar | `Tema.Buscar()` | `SELECT ... LIKE` |
| Buscar por ID | `Tema.BuscarPorId()` | `SELECT ... WHERE` |
| Modificar | `Tema.Actualizar()` | `UPDATE` |
| Eliminar | `Tema.Borrar()` | `DELETE` |

## Protección al eliminar temas

La tabla `palabras` contiene una clave externa hacia `temas`:

```sql
FOREIGN KEY (tema_id)
    REFERENCES temas(id)
    ON DELETE RESTRICT
```

Además de esta protección de SQLite, el programa cuenta primero las palabras asociadas mediante `Tema.ContarPalabras()`.

Si el tema contiene palabras, se muestra un mensaje claro y no se intenta eliminar:

```text
No se puede eliminar el tema porque contiene palabras.
Primero debes eliminar esas palabras o cambiarlas de tema.
```

## Colores del juego

La clase `JuegoAhorcado` utiliza `Console.ForegroundColor` y restaura siempre el color anterior.

- Verde: pocos errores y victoria.
- Amarillo: peligro intermedio.
- Rojo: muchos errores o derrota.
- Cian: palabra oculta y solución.
- Magenta: pista.
- Amarillo oscuro: letras utilizadas.

El color del dibujo se decide de una forma muy sencilla:

```csharp
ConsoleColor color = ConsoleColor.Green;

if (errores >= 3)
{
    color = ConsoleColor.Yellow;
}

if (errores >= 5)
{
    color = ConsoleColor.Red;
}
```

## Mapeo de tablas a objetos

Cada fila de `temas` se convierte en un objeto `Tema`, y cada fila de `palabras` se convierte en un objeto `Palabra`.

Una palabra contiene directamente su objeto tema:

```csharp
public Tema Tema
{
    get { return tema; }
    set { tema = value; }
}
```

Al leer una palabra de SQLite se construyen ambos objetos:

```csharp
Tema tema = new Tema(
    temaId,
    nombreTema,
    descripcionTema);

Palabra palabra = new Palabra(
    palabraId,
    texto,
    pista,
    tema);
```

## Consultas parametrizadas

Los datos del usuario se añaden mediante parámetros y `AddWithValue`, evitando concatenarlos directamente dentro del SQL:

```csharp
string sql =
    "INSERT INTO temas " +
    "(nombre, descripcion) " +
    "VALUES (@nombre, @descripcion)";

SqliteCommand cmd = new SqliteCommand(sql, conexion);
cmd.Parameters.AddWithValue("@nombre", nombre.Trim());
cmd.Parameters.AddWithValue("@descripcion", descripcion.Trim());
```

## Estructura de ficheros

```text
Ahorcado/
├── Program.cs
├── BaseDatos.cs
├── Tema.cs
├── Palabra.cs
├── GestorTemas.cs
├── GestorPalabras.cs
├── JuegoAhorcado.cs
├── TextoUtil.cs
├── ahorcado.db
└── README.md
```

## Base de datos

No es necesario modificar la base de datos existente. La tabla `temas` ya dispone de los campos necesarios:

```sql
CREATE TABLE temas (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL UNIQUE COLLATE NOCASE,
    descripcion TEXT NOT NULL
);
```

La restricción `UNIQUE COLLATE NOCASE` impide nombres repetidos que solo cambien en mayúsculas y minúsculas.

## Ejecución

El proyecto necesita el paquete:

```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.12" />
```

Después se puede ejecutar con:

```bash
dotnet restore
dotnet run
```

El fichero `ahorcado.db` debe encontrarse en la carpeta desde la que se ejecuta el programa o copiarse al directorio de salida.
