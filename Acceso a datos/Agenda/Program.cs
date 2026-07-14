using Microsoft.Data.Sqlite;

class ProgramaAgenda
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string cadenaConexion = "Data Source=agenda.db";
        SqliteConnection conexion = new SqliteConnection(cadenaConexion);

        using (conexion)
        {
            conexion.Open();

            CrearTabla(conexion);
        }
    }

    static void CrearTabla(SqliteConnection conexion)
    {
        string sqlCrearTabla = 
                "CREATE TABLE IF NOT EXISTS personas (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "nombre TEXT NOT NULL, " +
                "telefono TEXT NOT NULL)";

        using (SqliteCommand comando = new SqliteCommand(sqlCrearTabla, conexion))
        {
            comando.ExecuteNonQuery();
        }
    }
}