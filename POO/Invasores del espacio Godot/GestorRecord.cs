using Godot;

// ============================================================================
// GESTIÓN DEL RÉCORD
// ============================================================================
//
// Esta clase se responsabiliza exclusivamente de leer y escribir la puntuación
// máxima. Separar el acceso a ficheros de la clase Juego evita mezclar dos tareas
// diferentes:
//
// - Juego contiene las reglas de la partida.
// - GestorRecord contiene la persistencia de datos.
//
// El récord se guarda como un único número entero dentro de un fichero de texto.
// Es una solución sencilla, suficiente para este proyecto y fácil de inspeccionar
// manualmente durante el aprendizaje.
// ============================================================================

// La clase es static porque no necesita almacenar datos propios entre llamadas.
// No se crean objetos GestorRecord; se usan directamente sus métodos Cargar() y
// Guardar().
static class GestorRecord
{
    // user:// es una ruta especial de Godot que apunta a una carpeta en la que la
    // aplicación tiene permiso para escribir datos del usuario.
    //
    // No se utiliza res:// porque esa ruta representa los recursos del proyecto y
    // normalmente es de solo lectura cuando el juego está exportado.
    private const string NombreArchivo = "user://record.txt";

    // Intenta leer el récord almacenado y lo devuelve como int.
    //
    // Si el fichero todavía no existe, no puede abrirse o contiene un valor no
    // válido, se devuelve 0. Un problema con el récord no debe impedir que el
    // usuario pueda jugar.
    public static int Cargar()
    {
        // La primera vez que se ejecuta el juego todavía no existe record.txt.
        // En ese caso se considera que el récord inicial es cero.
        if (!FileAccess.FileExists(NombreArchivo))
        {
            return 0;
        }

        // FileAccess.Open abre el archivo en modo lectura.
        //
        // La instrucción using garantiza que el archivo se cierre y sus recursos
        // se liberen automáticamente al terminar el método, incluso si se produce
        // una salida anticipada mediante return.
        using FileAccess archivo = FileAccess.Open(
            NombreArchivo,
            FileAccess.ModeFlags.Read);

        // Si Godot no ha podido abrir el archivo, Open devuelve null.
        // Se responde con un récord 0 en lugar de detener el juego con un error.
        if (archivo == null)
        {
            return 0;
        }

        // GetAsText lee todo el contenido. Trim elimina espacios y saltos de línea
        // que podrían haberse añadido accidentalmente alrededor del número.
        string contenido = archivo.GetAsText().Trim();

        // TryParse intenta convertir el texto en un entero sin lanzar una excepción
        // si el contenido está vacío, dañado o ha sido modificado manualmente.
        if (int.TryParse(contenido, out int record))
        {
            return record;
        }

        // Cualquier contenido no válido se interpreta como ausencia de récord.
        return 0;
    }

    // Guarda el récord recibido en el fichero y devuelve si la operación se ha
    // realizado correctamente.
    //
    // - true: el fichero se ha abierto y el número se ha escrito.
    // - false: Godot no ha podido abrir el fichero para escritura.
    public static bool Guardar(int record)
    {
        // El modo Write crea el fichero si no existe y sustituye su contenido si
        // ya existía. No es necesario borrar manualmente el valor anterior.
        using FileAccess archivo = FileAccess.Open(
            NombreArchivo,
            FileAccess.ModeFlags.Write);

        // Una apertura fallida produce null. Se informa mediante false para que
        // Juego pueda mostrar un mensaje sin finalizar inesperadamente.
        if (archivo == null)
        {
            return false;
        }

        // StoreString escribe texto, por lo que el número se convierte primero a
        // string. El archivo contendrá únicamente un valor, por ejemplo: 1200.
        archivo.StoreString(record.ToString());

        return true;
    }
}
