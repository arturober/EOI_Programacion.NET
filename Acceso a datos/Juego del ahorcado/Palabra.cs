using Microsoft.Data.Sqlite;

class Palabra
{
    private int id;
    private string texto;
    private string pista;
    private Tema tema;

    public int Id
    {
        get { return id; }
    }

    public string Texto
    {
        get { return texto; }
        set { texto = value; }
    }

    public string Pista
    {
        get { return pista; }
        set { pista = value; }
    }

    // En la tabla se guarda tema_id, pero en C# resulta más cómodo guardar
    // el objeto Tema completo. Para obtener la clave externa usamos tema.Id.
    public Tema Tema
    {
        get { return tema; }
        set { tema = value; }
    }

    // Constructor para una palabra que todavía no se ha insertado.
    public Palabra(string texto, string pista, Tema tema)
    {
        this.texto = texto;
        this.pista = pista;
        this.tema = tema;
    }

    // Constructor para una palabra recuperada de la base de datos.
    public Palabra(
        int id,
        string texto,
        string pista,
        Tema tema)
    {
        this.id = id;
        this.texto = texto;
        this.pista = pista;
        this.tema = tema;
    }

    public override string ToString()
    {
        return "- " + texto + " (Id: " + id + ") [" + tema.Nombre + "] (" + pista + ")";
    }

    // CREATE: inserta en la tabla los datos del objeto actual.
    public bool Insertar(SqliteConnection conexion)
    {
        bool insertada = false;

        // Comprobamos primero el duplicado para poder mostrar al usuario
        // un mensaje comprensible antes de intentar ejecutar el INSERT.
        if (!Existe(conexion, texto))
        {
            string textoNormalizado =
                TextoUtil.NormalizarParaComparar(texto);

            string sql =
                "INSERT INTO palabras " +
                "(palabra, palabra_normalizada, pista, tema_id) " +
                "VALUES (@palabra, @normalizada, @pista, @temaId)";

            SqliteCommand cmd = new SqliteCommand(sql, conexion);

            using (cmd)
            {
                cmd.Parameters.AddWithValue(
                    "@palabra",
                    texto.Trim());
                cmd.Parameters.AddWithValue(
                    "@normalizada",
                    textoNormalizado);
                cmd.Parameters.AddWithValue(
                    "@pista",
                    pista.Trim());
                cmd.Parameters.AddWithValue(
                    "@temaId",
                    tema.Id);

                cmd.Prepare();

                int cantidad = cmd.ExecuteNonQuery();
                insertada = cantidad == 1;

                // Después de insertar, consultamos el ID que SQLite
                // acaba de generar para este nuevo registro.
                if (insertada)
                {
                    string sqlId =
                        "SELECT last_insert_rowid()";

                    SqliteCommand cmdId =
                        new SqliteCommand(sqlId, conexion);

                    using (cmdId)
                    {
                        object? resultado =
                            cmdId.ExecuteScalar();

                        id = Convert.ToInt32(resultado);
                    }
                }
            }
        }

        return insertada;
    }

    // UPDATE: actualiza la fila correspondiente al objeto actual.
    public bool Actualizar(SqliteConnection conexion)
    {
        bool actualizada = false;

        // Ignoramos el ID de la propia palabra para que no se considere
        // duplicada consigo misma durante una modificación.
        if (!Existe(conexion, texto, id))
        {
            string textoNormalizado =
                TextoUtil.NormalizarParaComparar(texto);

            string sql =
                "UPDATE palabras SET " +
                "palabra = @palabra, " +
                "palabra_normalizada = @normalizada, " +
                "pista = @pista, " +
                "tema_id = @temaId " +
                "WHERE id = @id";

            SqliteCommand cmd = new SqliteCommand(sql, conexion);

            using (cmd)
            {
                cmd.Parameters.AddWithValue(
                    "@palabra",
                    texto.Trim());
                cmd.Parameters.AddWithValue(
                    "@normalizada",
                    textoNormalizado);
                cmd.Parameters.AddWithValue(
                    "@pista",
                    pista.Trim());
                cmd.Parameters.AddWithValue(
                    "@temaId",
                    tema.Id);
                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                cmd.Prepare();

                int cantidad = cmd.ExecuteNonQuery();
                actualizada = cantidad == 1;
            }
        }

        return actualizada;
    }

    // DELETE: borra de la tabla la fila representada por este objeto.
    public bool Borrar(SqliteConnection conexion)
    {
        bool borrada = false;

        string sql =
            "DELETE FROM palabras " +
            "WHERE id = @id";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            int cantidad = cmd.ExecuteNonQuery();
            borrada = cantidad == 1;
        }

        return borrada;
    }

    // READ: devuelve todas las palabras junto con los datos de su tema.
    public static List<Palabra> Listar(
        SqliteConnection conexion)
    {
        List<Palabra> palabras = new List<Palabra>();

        // Relacionamos las tablas mediante WHERE, tal como se ha decidido.
        string sql =
            "SELECT p.id AS palabra_id, p.palabra, p.pista, " +
            "t.id AS tema_id, t.nombre, t.descripcion " +
            "FROM palabras p, temas t " +
            "WHERE p.tema_id = t.id " +
            "ORDER BY p.palabra";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            SqliteDataReader lector = cmd.ExecuteReader();

            using (lector)
            {
                while (lector.Read())
                {
                    Palabra palabra = CrearDesdeLector(lector);
                    palabras.Add(palabra);
                }
            }
        }

        return palabras;
    }

    // READ: busca palabras cuyo texto contenga lo escrito por el usuario.
    public static List<Palabra> Buscar(
        SqliteConnection conexion,
        string textoBuscado)
    {
        List<Palabra> palabras = new List<Palabra>();
        string textoNormalizado =
            TextoUtil.NormalizarParaComparar(textoBuscado);

        string sql =
            "SELECT p.id AS palabra_id, p.palabra, p.pista, " +
            "t.id AS tema_id, t.nombre, t.descripcion " +
            "FROM palabras p, temas t " +
            "WHERE p.tema_id = t.id " +
            "AND p.palabra_normalizada LIKE @texto " +
            "ORDER BY p.palabra";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            cmd.Parameters.AddWithValue(
                "@texto",
                "%" + textoNormalizado + "%");

            SqliteDataReader lector = cmd.ExecuteReader();

            using (lector)
            {
                while (lector.Read())
                {
                    Palabra palabra = CrearDesdeLector(lector);
                    palabras.Add(palabra);
                }
            }
        }

        return palabras;
    }

    // READ: busca una sola palabra a partir de su clave primaria.
    public static Palabra? BuscarPorId(
        SqliteConnection conexion,
        int idBuscado)
    {
        Palabra? palabraEncontrada = null;

        string sql =
            "SELECT p.id AS palabra_id, p.palabra, p.pista, " +
            "t.id AS tema_id, t.nombre, t.descripcion " +
            "FROM palabras p, temas t " +
            "WHERE p.tema_id = t.id " +
            "AND p.id = @id";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            cmd.Parameters.AddWithValue("@id", idBuscado);

            SqliteDataReader lector = cmd.ExecuteReader();

            using (lector)
            {
                if (lector.Read())
                {
                    palabraEncontrada = CrearDesdeLector(lector);
                }
            }
        }

        return palabraEncontrada;
    }

    // Comprueba si ya existe una palabra equivalente.
    // idIgnorado se utiliza al modificar para excluir el propio registro.
    public static bool Existe(
        SqliteConnection conexion,
        string texto,
        int idIgnorado = 0)
    {
        string textoNormalizado =
            TextoUtil.NormalizarParaComparar(texto);

        string sql =
            "SELECT COUNT(*) " +
            "FROM palabras " +
            "WHERE palabra_normalizada = @normalizada " +
            "AND id <> @idIgnorado";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);
        long cantidad = 0;

        using (cmd)
        {
            cmd.Parameters.AddWithValue(
                "@normalizada",
                textoNormalizado);
            cmd.Parameters.AddWithValue(
                "@idIgnorado",
                idIgnorado);

            cmd.Prepare();

            object? resultado = cmd.ExecuteScalar();
            cantidad = Convert.ToInt64(resultado);
        }

        return cantidad > 0;
    }

    // Elige una palabra al azar. Si temaId vale 0, se permiten todos los temas.
    public static Palabra? ObtenerAleatoria(
        SqliteConnection conexion,
        int temaId)
    {
        Palabra? palabraEncontrada = null;

        string sql =
            "SELECT p.id AS palabra_id, p.palabra, p.pista, " +
            "t.id AS tema_id, t.nombre, t.descripcion " +
            "FROM palabras p, temas t " +
            "WHERE p.tema_id = t.id ";

        if (temaId > 0)
        {
            sql += "AND p.tema_id = @temaId ";
        }

        // Esta forma no es la más eficiente para millones de registros,
        // pero es muy fácil de entender y adecuada para este proyecto.
        sql += "ORDER BY RANDOM() LIMIT 1";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);

        using (cmd)
        {
            if (temaId > 0)
            {
                cmd.Parameters.AddWithValue("@temaId", temaId);
            }

            SqliteDataReader lector = cmd.ExecuteReader();

            using (lector)
            {
                if (lector.Read())
                {
                    palabraEncontrada = CrearDesdeLector(lector);
                }
            }
        }

        return palabraEncontrada;
    }

    // Cuenta cuántas palabras pertenecen a un tema concreto.
    public static int ContarPorTema(
        SqliteConnection conexion,
        int temaId)
    {
        string sql =
            "SELECT COUNT(*) " +
            "FROM palabras " +
            "WHERE tema_id = @temaId";

        SqliteCommand cmd = new SqliteCommand(sql, conexion);
        int cantidad = 0;

        using (cmd)
        {
            cmd.Parameters.AddWithValue("@temaId", temaId);
            object? resultado = cmd.ExecuteScalar();
            cantidad = Convert.ToInt32(resultado);
        }

        return cantidad;
    }

    // Convierte la fila actual del lector en objetos Tema y Palabra.
    // Así evitamos repetir este mismo código en todos los SELECT.
    private static Palabra CrearDesdeLector(
        SqliteDataReader lector)
    {
        int temaId = Convert.ToInt32(lector["tema_id"]);
        string nombreTema = Convert.ToString(
            lector["nombre"]) ?? "";
        string descripcionTema = Convert.ToString(
            lector["descripcion"]) ?? "";

        Tema tema = new Tema(
            temaId,
            nombreTema,
            descripcionTema);

        int palabraId = Convert.ToInt32(
            lector["palabra_id"]);
        string texto = Convert.ToString(
            lector["palabra"]) ?? "";
        string pista = Convert.ToString(
            lector["pista"]) ?? "";

        Palabra palabra = new Palabra(
            palabraId,
            texto,
            pista,
            tema);

        return palabra;
    }
}
