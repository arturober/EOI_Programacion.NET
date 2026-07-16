using Microsoft.Data.Sqlite;

class BaseDatos
{
    // La base de datos se encuentra en un único fichero llamado ahorcado.db.
    // Se buscará en la misma carpeta desde la que se ejecute el programa.
    private const string CadenaConexion = "Data Source=ahorcado.db";

    public static SqliteConnection CrearConexion()
    {
        SqliteConnection conexion = new SqliteConnection(CadenaConexion);
        return conexion;
    }

    public static void CrearTablas(SqliteConnection conexion)
    {
        // SQLite no activa siempre las claves externas automáticamente.
        // Esta orden hace que se respete la relación entre palabras y temas.
        string sqlClavesExternas = "PRAGMA foreign_keys = ON";
        SqliteCommand cmdClaves = new SqliteCommand(
            sqlClavesExternas,
            conexion);

        using (cmdClaves)
        {
            cmdClaves.ExecuteNonQuery();
        }

        // Cada fila de esta tabla se representa mediante un objeto Tema.
        string sqlTemas =
            "CREATE TABLE IF NOT EXISTS temas (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "nombre TEXT NOT NULL UNIQUE COLLATE NOCASE, " +
            "descripcion TEXT NOT NULL)";

        SqliteCommand cmdTemas = new SqliteCommand(
            sqlTemas,
            conexion);

        using (cmdTemas)
        {
            cmdTemas.ExecuteNonQuery();
        }

        // Cada fila de esta tabla se representa mediante un objeto Palabra.
        // palabra_normalizada guarda el texto en minúsculas y sin tildes.
        // Su restricción UNIQUE impide duplicados como "camión" y "CAMION".
        string sqlPalabras =
            "CREATE TABLE IF NOT EXISTS palabras (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "palabra TEXT NOT NULL, " +
            "palabra_normalizada TEXT NOT NULL UNIQUE, " +
            "pista TEXT NOT NULL, " +
            "tema_id INTEGER NOT NULL, " +
            "FOREIGN KEY (tema_id) REFERENCES temas(id) ON DELETE RESTRICT)";

        SqliteCommand cmdPalabras = new SqliteCommand(
            sqlPalabras,
            conexion);

        using (cmdPalabras)
        {
            cmdPalabras.ExecuteNonQuery();
        }

        // Este índice ayuda a localizar las palabras de un tema.
        // No es imprescindible, pero tampoco complica el código.
        string sqlIndice =
            "CREATE INDEX IF NOT EXISTS idx_palabras_tema " +
            "ON palabras (tema_id)";

        SqliteCommand cmdIndice = new SqliteCommand(
            sqlIndice,
            conexion);

        using (cmdIndice)
        {
            cmdIndice.ExecuteNonQuery();
        }
    }
}
