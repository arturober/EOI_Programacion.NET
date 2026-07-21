using Microsoft.Data.Sqlite;

namespace ListaTareas.Datos;

// Esta clase se ocupa únicamente de abrir la conexión con SQLite.
public static class BaseDatos
{
    public static SqliteConnection Inicializar()
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "lista_tareas.db");
        string cadenaConexion = $"Data Source={ruta};Foreign Keys=True";

        SqliteConnection conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        return conexion;
    }
}
