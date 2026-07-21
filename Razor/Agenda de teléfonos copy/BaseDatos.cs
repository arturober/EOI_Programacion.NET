using Microsoft.Data.Sqlite;

namespace AgendaTelefonos.Datos;

// Esta clase se ocupa únicamente de abrir la conexión con SQLite.
public static class BaseDatos
{
    public static SqliteConnection Inicializar()
    {
        string ruta = Path.Combine(Directory.GetCurrentDirectory(), "agenda.db");
        string cadenaConexion = $"Data Source={ruta}";

        SqliteConnection conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();
        return conexion;
    }
}
