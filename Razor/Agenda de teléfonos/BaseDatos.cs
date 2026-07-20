using Microsoft.Data.Sqlite;

public static class BaseDatos
{
    private static string rutaBD = "Data Source=agenda.db";


    public static SqliteConnection Inicializar()
    {
        SqliteConnection conexion = new SqliteConnection(rutaBD);
        conexion.Open();
        return conexion;
    }
}