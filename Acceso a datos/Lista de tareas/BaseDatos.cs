using Microsoft.Data.Sqlite;

class BaseDatos
{
    private const string CadenaConexion = "Data Source=lista_tareas.db";

    public static SqliteConnection CrearConexion()
    {
        SqliteConnection conexion = new SqliteConnection(CadenaConexion);
        return conexion;
    }
}