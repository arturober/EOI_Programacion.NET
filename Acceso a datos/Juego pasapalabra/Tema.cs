using Microsoft.Data.Sqlite;

class Tema
{
    private int id;
    private string nombre;
    private string descripcion;

    public int Id
    {
        get { return id; }
    }

    public string Nombre
    {
        get { return nombre; }
        set { nombre = value; }
    }

    public string Descripcion
    {
        get { return descripcion; }
        set { descripcion = value; }
    }
    
    public Tema(int id, string nombre, string descripcion)
    {
        this.id = id;
        this.nombre = nombre;
        this.descripcion = descripcion;
    }

    public Tema(string nombre, string descripcion)
    {
        this.nombre = nombre;
        this.descripcion = descripcion;
    }

    public override string ToString()
    {
        return $"{id}. {nombre} - {descripcion}";
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO temas (nombre, descripcion) VALUES (@nombre, @descripcion)";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@descripcion", descripcion);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE temas SET nombre = @nombre, descripcion = @descripcion WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@descripcion", descripcion);
            comando.Parameters.AddWithValue("@id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM temas WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public static List<Tema> Listar(SqliteConnection conexion)
    {
        List<Tema> temas = new List<Tema>();

        string sql = "SELECT id, nombre, descripcion FROM temas ORDER BY id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    temas.Add(CrearDesdeLector(lector));
                }
            }
        }

        return temas;
    }

    public static Tema? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT id, nombre, descripcion FROM temas WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@id", id);

            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                if (lector.Read())
                {
                    return CrearDesdeLector(lector);
                }
            }
        }

        return null;
    }

    public static List<Tema> Buscar(SqliteConnection conexion, string texto)
    {
        List<Tema> temas = new List<Tema>();

        string sql = "SELECT id, nombre, descripcion FROM temas WHERE nombre LIKE @nombre ORDER BY nombre";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", "%" + texto + "%");

            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    temas.Add(CrearDesdeLector(lector));
                }
            }
        }

        return temas;
    }

    public static bool Existe(SqliteConnection conexion, string nombre, int IdIgnorado = 0)
    {
        string sql = "SELECT COUNT(*) FROM temas WHERE nombre = @nombre COLLATE NOCASE AND id <> @idIgnorado";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@idIgnorado", IdIgnorado);
            long count = (long)(comando.ExecuteScalar() ?? 0);
            return count > 0;
        }
    }

    private static Tema CrearDesdeLector(SqliteDataReader lector)
    {
        int id = Convert.ToInt32(lector["id"]);
        string nombre = Convert.ToString(lector["nombre"]) ?? "";
        string descripcion = Convert.ToString(lector["descripcion"]) ?? "";

        return new Tema(id, nombre, descripcion);
    }
}