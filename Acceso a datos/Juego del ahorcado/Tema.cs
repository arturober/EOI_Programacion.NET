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

    // Constructor para un tema que todavía no se ha insertado.
    public Tema(string nombre, string descripcion)
    {
        this.nombre = nombre;
        this.descripcion = descripcion;
    }

    // Constructor para un tema recuperado de la base de datos.
    public Tema(int id, string nombre, string descripcion)
    {
        this.id = id;
        this.nombre = nombre;
        this.descripcion = descripcion;
    }

    public override string ToString()
    {
        return id + ". " + nombre;
    }

    // CREATE: inserta en la tabla los datos del objeto actual.
    public bool Insertar(SqliteConnection conexion)
    {
        bool insertado = false;

        if (!Existe(conexion, nombre))
        {
            string sql =
                "INSERT INTO temas " +
                "(nombre, descripcion) " +
                "VALUES (@nombre, @descripcion)";

            SqliteCommand cmd = new SqliteCommand(sql, conexion);

            using (cmd)
            {
                cmd.Parameters.AddWithValue(
                    "@nombre",
                    nombre.Trim());
                cmd.Parameters.AddWithValue(
                    "@descripcion",
                    descripcion.Trim());

                cmd.Prepare();

                int cantidad = cmd.ExecuteNonQuery();
                insertado = cantidad == 1;

                // Guardamos en el objeto el ID generado por SQLite.
                if (insertado)
                {
                    string sqlId = "SELECT last_insert_rowid()";
                    SqliteCommand cmdId =
                        new SqliteCommand(sqlId, conexion);

                    using (cmdId)
                    {
                        object? resultado = cmdId.ExecuteScalar();
                        id = Convert.ToInt32(resultado);
                    }
                }
            }
        }

        return insertado;
    }

    // UPDATE: actualiza la fila correspondiente al objeto actual.
    public bool Actualizar(SqliteConnection conexion)
    {
        bool actualizado = false;

        // Ignoramos el propio ID para que el tema no sea duplicado de sí mismo.
        if (!Existe(conexion, nombre, id))
        {
            string sql =
                "UPDATE temas SET " +
                "nombre = @nombre, " +
                "descripcion = @descripcion " +
                "WHERE id = @id";

            SqliteCommand cmd = new SqliteCommand(sql, conexion);

            using (cmd)
            {
                cmd.Parameters.AddWithValue(
                    "@nombre",
                    nombre.Trim());
                cmd.Parameters.AddWithValue(
                    "@descripcion",
                    descripcion.Trim());
                cmd.Parameters.AddWithValue("@id", id);

                cmd.Prepare();

                int cantidad = cmd.ExecuteNonQuery();
                actualizado = cantidad == 1;
            }
        }

        return actualizado;
    }

    // DELETE: solo elimina el tema si no contiene ninguna palabra.
    public bool Borrar(SqliteConnection conexion)
    {
        bool borrado = false;

        if (ContarPalabras(conexion) == 0)
        {
            string sql =
                "DELETE FROM temas " +
                "WHERE id = @id";

            SqliteCommand cmd = new SqliteCommand(sql, conexion);

            using (cmd)
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();

                int cantidad = cmd.ExecuteNonQuery();
                borrado = cantidad == 1;
            }
        }

        return borrado;
    }

    // READ: devuelve todos los temas convertidos en objetos Tema.
    public static List<Tema> Listar(SqliteConnection conexion)
    {
        List<Tema> temas = new List<Tema>();

        string sql =
            "SELECT id, nombre, descripcion " +
            "FROM temas " +
            "ORDER BY nombre";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            SqliteDataReader lector = cmd.ExecuteReader();

            using (lector)
            {
                while (lector.Read())
                {
                    Tema tema = CrearDesdeLector(lector);
                    temas.Add(tema);
                }
            }
        }

        return temas;
    }

    // READ: busca temas por su nombre o por su descripción.
    public static List<Tema> Buscar(
        SqliteConnection conexion,
        string textoBuscado)
    {
        List<Tema> temas = new List<Tema>();

        string sql =
            "SELECT id, nombre, descripcion " +
            "FROM temas " +
            "WHERE nombre LIKE @texto " +
            "OR descripcion LIKE @texto " +
            "ORDER BY nombre";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            cmd.Parameters.AddWithValue(
                "@texto",
                "%" + textoBuscado.Trim() + "%");

            SqliteDataReader lector = cmd.ExecuteReader();

            using (lector)
            {
                while (lector.Read())
                {
                    Tema tema = CrearDesdeLector(lector);
                    temas.Add(tema);
                }
            }
        }

        return temas;
    }

    // READ: busca un tema concreto por su clave primaria.
    public static Tema? BuscarPorId(
        SqliteConnection conexion,
        int idBuscado)
    {
        Tema? temaEncontrado = null;

        string sql =
            "SELECT id, nombre, descripcion " +
            "FROM temas " +
            "WHERE id = @id";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            cmd.Parameters.AddWithValue("@id", idBuscado);

            SqliteDataReader lector = cmd.ExecuteReader();

            using (lector)
            {
                if (lector.Read())
                {
                    temaEncontrado = CrearDesdeLector(lector);
                }
            }
        }

        return temaEncontrado;
    }

    // Comprueba si ya existe otro tema con el mismo nombre.
    public static bool Existe(
        SqliteConnection conexion,
        string nombre,
        int idIgnorado = 0)
    {
        string sql =
            "SELECT COUNT(*) " +
            "FROM temas " +
            "WHERE nombre = @nombre COLLATE NOCASE " +
            "AND id <> @idIgnorado";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);
        long cantidad = 0;

        using (cmd)
        {
            cmd.Parameters.AddWithValue(
                "@nombre",
                nombre.Trim());
            cmd.Parameters.AddWithValue(
                "@idIgnorado",
                idIgnorado);

            cmd.Prepare();

            object? resultado = cmd.ExecuteScalar();
            cantidad = Convert.ToInt64(resultado);
        }

        return cantidad > 0;
    }

    // Cuenta las palabras relacionadas con el tema actual.
    public int ContarPalabras(SqliteConnection conexion)
    {
        string sql =
            "SELECT COUNT(*) " +
            "FROM palabras " +
            "WHERE tema_id = @temaId";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);
        int cantidad = 0;

        using (cmd)
        {
            cmd.Parameters.AddWithValue("@temaId", id);
            object? resultado = cmd.ExecuteScalar();
            cantidad = Convert.ToInt32(resultado);
        }

        return cantidad;
    }

    // Convierte la fila actual del lector en un objeto Tema.
    private static Tema CrearDesdeLector(SqliteDataReader lector)
    {
        int id = Convert.ToInt32(lector["id"]);
        string nombre = Convert.ToString(
            lector["nombre"]) ?? "";
        string descripcion = Convert.ToString(
            lector["descripcion"]) ?? "";

        Tema tema = new Tema(
            id,
            nombre,
            descripcion);

        return tema;
    }
}
