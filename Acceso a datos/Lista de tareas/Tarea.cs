using Microsoft.Data.Sqlite;

class Tarea
{
    private int id;
    private string titulo;
    private string descripcion;
    private bool completada;
    private Categoria categoria;

    public int Id { 
        get { return id; } 
    }
    public string Titulo { 
        get { return titulo; } 
        set { titulo = value; }
    }
    public string Descripcion { 
        get { return descripcion; } 
        set { descripcion = value; }
    }
    public bool Completada { 
        get { return completada; } 
        set { completada = value; }
    }
    public Categoria Categoria { 
        get { return categoria; } 
        set { categoria = value; }
    }

    public Tarea(int id, string titulo, string descripcion, bool completada, Categoria categoria)
    {
        this.id = id;
        this.titulo = titulo;
        this.descripcion = descripcion;
        this.completada = completada;
        this.categoria = categoria;
    }

    public Tarea(string titulo, string descripcion, bool completada, Categoria categoria)
    {
        this.titulo = titulo;
        this.descripcion = descripcion;
        this.completada = completada;
        this.categoria = categoria;
    }

    public override string ToString()
    {
        string estado = completada ? "[X]" : "[ ]";

        return $"{estado} {id}. {titulo} - {descripcion} - ({categoria.Nombre})";
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO tareas (titulo, descripcion, completada, categoria_id) VALUES (@titulo, @descripcion, @completada, @categoria_id)";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@titulo", titulo.Trim());
            comando.Parameters.AddWithValue("@descripcion", descripcion.Trim());
            comando.Parameters.AddWithValue("@completada", completada ? 1 : 0);
            comando.Parameters.AddWithValue("@categoria_id", categoria.Id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE tareas SET titulo = @titulo, descripcion = @descripcion, completada = @completada, categoria_id = @categoria_id WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@titulo", titulo.Trim());
            comando.Parameters.AddWithValue("@descripcion", descripcion.Trim());
            comando.Parameters.AddWithValue("@completada", completada ? 1 : 0);
            comando.Parameters.AddWithValue("@categoria_id", categoria.Id);
            comando.Parameters.AddWithValue("@id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }
    
    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM tareas WHERE id = @id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public static List<Tarea> Listar(SqliteConnection conexion)
    {
        List<Tarea> tareas = new List<Tarea>();

        string sql = "SELECT t.id AS tarea_id, t.titulo, t.descripcion, t.completada, c.id AS categoria_id, c.nombre AS categoria_nombre, c.descripcion AS categoria_descripcion " +
                     "FROM tareas t, categorias c " +
                     "WHERE t.categoria_id = c.id " +
                     "ORDER BY t.completada, t.id";

        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    Tarea tarea = CrearDesdeLector(lector);
                    tareas.Add(tarea);
                }
            }
        }

        return tareas;
    }

    private static Tarea CrearDesdeLector(SqliteDataReader lector)
    {
        int id = Convert.ToInt32(lector["tarea_id"]);
        string titulo = Convert.ToString(lector["titulo"]) ?? "";
        string descripcion = Convert.ToString(lector["descripcion"]) ?? "";
        bool completada = Convert.ToBoolean(lector["completada"]);
        int categoriaId = Convert.ToInt32(lector["categoria_id"]);
        string categoriaNombre = Convert.ToString(lector["categoria_nombre"]) ?? "";
        string categoriaDescripcion = Convert.ToString(lector["categoria_descripcion"]) ?? "";

        Categoria categoria = new Categoria(categoriaId, categoriaNombre, categoriaDescripcion);
        return new Tarea(id, titulo, descripcion, completada, categoria);
    }

    public static Tarea? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT t.id AS tarea_id, t.titulo, t.descripcion, t.completada, c.id AS categoria_id, c.nombre AS categoria_nombre, c.descripcion AS categoria_descripcion " +
                     "FROM tareas t, categorias c " +
                     "WHERE t.id = @id AND t.categoria_id = c.id";

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
}