using Microsoft.Data.Sqlite;

class BaseDatos
{
    private static string rutaBaseDatos = "Data Source=pasapalabra.db";

    public static SqliteConnection CrearConexion()
    {
        return new SqliteConnection(rutaBaseDatos);
    }
}