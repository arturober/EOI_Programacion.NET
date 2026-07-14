using Microsoft.Data.Sqlite;

class BaseDatos
{
    private const string CadenaConexion = "Data Source=ahorcado.db";

    public void Preparar()
    {
        using (SqliteConnection conexion = new SqliteConnection(CadenaConexion))
        {
            conexion.Open();

            CrearTablas(conexion);

            if (ContarPalabras(conexion) == 0)
            {
                InsertarPalabrasIniciales(conexion);
            }
        }
    }

    private void CrearTablas(SqliteConnection conexion)
    {
        string sqlCrearTabla = "CREATE TABLE IF NOT EXISTS palabras (" +
                                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                "texto TEXT NOT NULL, " +
                                "pista TEXT NOT NULL)";

        using (SqliteCommand comando = new SqliteCommand(sqlCrearTabla, conexion))
        {
            comando.ExecuteNonQuery();
        }
    }

    private void InsertarPalabrasIniciales(SqliteConnection conexion)
    {
        Palabra[] palabras = 
        {
            new("programacion", "Actividad de escribir código"),
            new("ahorcado", "Juego de adivinanza de palabras"),
            new("ordenador", "Dispositivo electrónico para procesar información"),
            new("desarrollo", "Proceso de creación de software"),
            new("algoritmo", "Conjunto de instrucciones para resolver un problema")
        };

        foreach (Palabra palabra in palabras)
        {
            string sqlInsertar = "INSERT INTO palabras (texto, pista) VALUES (@texto, @pista)";

            using (SqliteCommand comando = new SqliteCommand(sqlInsertar, conexion))
            {
                comando.Parameters.AddWithValue("@texto", palabra.Texto);
                comando.Parameters.AddWithValue("@pista", palabra.Pista);
                comando.ExecuteNonQuery();
            }
        }
    }

    private int ContarPalabras(SqliteConnection conexion)
    {
        string sqlContar = "SELECT COUNT(*) FROM palabras";

        using (SqliteCommand comando = new SqliteCommand(sqlContar, conexion))
        {
            return Convert.ToInt32(comando.ExecuteScalar());
        }
    }
}