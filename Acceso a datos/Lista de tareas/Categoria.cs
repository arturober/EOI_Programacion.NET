using Microsoft.Data.Sqlite;

class Categoria
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

    public Categoria(string nombre, string descripcion)
    {
        this.nombre = nombre;
        this.descripcion = descripcion;
    }

    public Categoria(int id, string nombre, string descripcion)
    {
        this.id = id;
        this.nombre = nombre;
        this.descripcion = descripcion;
    }

    public override string ToString()
    {
        return $"{id}. {nombre} - {descripcion}";
    }

    public static bool Existe(SqliteConnection conexion, string nombre, int id = 0)
    {
        string sql = "SELECT COUNT(*) FROM categorias WHERE nombre = @nombre COLLATE NOCASE AND id <> @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", nombre.Trim());
            comando.Parameters.AddWithValue("@id", id);
            long count = (long)(comando.ExecuteScalar() ?? 0);
            return count > 0;
        }
    }

    public bool Insertar(SqliteConnection conexion)
    {
        bool insertada = false;

        if (!Existe(conexion, nombre))
        {
            string sql = "INSERT INTO categorias (nombre, descripcion) VALUES (@nombre, @descripcion)";

            using (SqliteCommand comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@nombre", nombre.Trim());
                comando.Parameters.AddWithValue("@descripcion", descripcion.Trim());
                int filasAfectadas = comando.ExecuteNonQuery();
                insertada = filasAfectadas > 0;
            }
        }
        return insertada;
    }

    public bool Borrar(SqliteConnection conexion)
    {
        bool borrada = false;

        string sql = "DELETE FROM categorias WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@id", id);
            int filasAfectadas = comando.ExecuteNonQuery();
            borrada = filasAfectadas > 0;
        }

        return borrada;
    }

    public static List<Categoria> Listar(SqliteConnection conexion)
    {
        List<Categoria> categorias = new List<Categoria>();

        string sql = "SELECT id, nombre, descripcion FROM categorias ORDER BY nombre";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    categorias.Add(CrearDesdeLector(lector));
                }
            }
        }

        return categorias;
    }

    private static Categoria CrearDesdeLector(SqliteDataReader lector)
    {
        int id = Convert.ToInt32(lector["id"]);
        string nombre = Convert.ToString(lector["nombre"]) ?? "";
        string descripcion = Convert.ToString(lector["descripcion"]) ?? "";

        return new Categoria(id, nombre, descripcion);
    }

    public static Categoria? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT id, nombre, descripcion FROM categorias WHERE id = @id";

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

    public bool Actualizar(SqliteConnection conexion)
    {
        bool actualizada = false;

        string sql = "UPDATE categorias SET nombre = @nombre, descripcion = @descripcion WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@nombre", nombre.Trim());
            comando.Parameters.AddWithValue("@descripcion", descripcion.Trim());
            comando.Parameters.AddWithValue("@id", id);
            int filasAfectadas = comando.ExecuteNonQuery();
            actualizada = filasAfectadas > 0;
        }

        return actualizada;
    }
}