using System;
using System.IO;

// ============================================================================
// GESTIÓN DEL RÉCORD
// ============================================================================
//
// Esta clase se responsabiliza exclusivamente de leer y escribir el récord.
// Separar esta tarea evita mezclar el acceso a ficheros con las reglas del juego.
//
// El archivo se crea en la carpeta de trabajo de la aplicación y contiene un
// único número entero. Es una forma muy sencilla de practicar persistencia.
// ============================================================================

static class GestorRecord
{
    private const string NombreArchivo = "record.txt";

    public static int Cargar()
    {
        try
        {
            // Durante la primera ejecución el archivo todavía no existe.
            // En ese caso se considera que el récord inicial es cero.
            if (!File.Exists(NombreArchivo))
            {
                return 0;
            }

            string contenido = File.ReadAllText(NombreArchivo);

            // TryParse evita que el programa falle si el archivo está vacío,
            // dañado o contiene un texto que no se puede convertir a entero.
            if (int.TryParse(contenido, out int record))
            {
                return record;
            }

            return 0;
        }
        catch (IOException)
        {
            // Un problema de lectura no debe impedir jugar. Si el archivo está
            // bloqueado o no puede leerse, se utiliza temporalmente un récord 0.
            return 0;
        }
        catch (UnauthorizedAccessException)
        {
            // También se evita que la aplicación termine si no tiene permisos
            // para acceder al archivo en la carpeta actual.
            return 0;
        }
    }

    public static bool Guardar(int record)
    {
        try
        {
            // WriteAllText crea el archivo si no existe y sustituye su contenido
            // si ya había sido creado en una partida anterior.
            File.WriteAllText(NombreArchivo, record.ToString());
            return true;
        }
        catch (IOException)
        {
            // Se devuelve false para que el juego pueda informar del problema
            // sin finalizar de forma inesperada.
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
