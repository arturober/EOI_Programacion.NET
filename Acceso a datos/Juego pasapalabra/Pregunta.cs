using Microsoft.Data.Sqlite;

class Pregunta
{
    private int id;
    private char letra;
    private string respuesta;
    private string definicion;
    private Tema tema;

    public int Id { 
        get { return id; } 
    }
    public char Letra { 
        get { return letra; } 
        set { letra = value; }
    }

    public string Respuesta { 
        get { return respuesta; } 
        set { respuesta = value; }
    }
    public string Definicion { 
        get { return definicion; } 
        set { definicion = value; }
    }
    public Tema Tema { 
        get { return tema; } 
        set { tema = value; }
    }

    public Pregunta(int id, char letra, string respuesta, string definicion, Tema tema)
    {
        this.id = id;
        this.letra = letra;
        this.respuesta = respuesta;
        this.definicion = definicion;
        this.tema = tema;
    }

    public Pregunta(char letra, string respuesta, string definicion, Tema tema)
    {
        this.letra = letra;
        this.respuesta = respuesta;
        this.definicion = definicion;
        this.tema = tema;
    }

    public override string ToString()
    {
        return $"{id}. {letra} - {respuesta} - {definicion} - {tema.Nombre}";
    }

    public string ObtenerEnunciado()
    {
        string respuestaNormalizada = TextoUtil.NormalizarParaComparar(respuesta);
        char letraNormalizada = TextoUtil.NormalizarCaracter(letra);

        string comienzo = "Contiene la letra '" + letraNormalizada + "': ";
        if (respuestaNormalizada.StartsWith(letraNormalizada.ToString()))
        {
            comienzo = "Comienza con la letra '" + letraNormalizada + "': ";
        }
        return comienzo + definicion;
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO preguntas (letra, respuesta, respuesta_normalizada, definicion, tema_id) VALUES (@letra, @respuesta, @respuesta_normalizada, @definicion, @tema_id)";
        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@letra", letra.ToString());
            comando.Parameters.AddWithValue("@respuesta", respuesta);
            comando.Parameters.AddWithValue("@respuesta_normalizada", TextoUtil.NormalizarParaComparar(respuesta));
            comando.Parameters.AddWithValue("@definicion", definicion);
            comando.Parameters.AddWithValue("@tema_id", tema.Id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public static List<Pregunta> Listar(SqliteConnection conexion)
    {
        List<Pregunta> preguntas = new List<Pregunta>();

        string sql = ConsultaBase() + " ORDER BY t.nombre, p.letra, p.respuesta";
        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            using (SqliteDataReader reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    Pregunta? pregunta = CrearDesdeReader(reader);
                    if (pregunta != null)
                    {
                        preguntas.Add(pregunta);
                    }
                }
            }
        }
        return preguntas;
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE preguntas SET letra = @letra, respuesta = @respuesta, respuesta_normalizada = @respuesta_normalizada, definicion = @definicion, tema_id = @tema_id WHERE id = @id";
        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@letra", letra.ToString());
            comando.Parameters.AddWithValue("@respuesta", respuesta);
            comando.Parameters.AddWithValue("@respuesta_normalizada", TextoUtil.NormalizarParaComparar(respuesta));
            comando.Parameters.AddWithValue("@definicion", definicion);
            comando.Parameters.AddWithValue("@tema_id", tema.Id);
            comando.Parameters.AddWithValue("@id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM preguntas WHERE id = @id";
        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }

    public static bool Existe(SqliteConnection conexion, string respuesta, int temaId, int idIgnorado = 0)
    {
        string sql = "SELECT COUNT(*) FROM preguntas WHERE respuesta_normalizada = @respuesta_normalizada AND tema_id = @tema_id AND id <> @idIgnorado";
        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@respuesta_normalizada", TextoUtil.NormalizarParaComparar(respuesta));
            comando.Parameters.AddWithValue("@tema_id", temaId);
            comando.Parameters.AddWithValue("@idIgnorado", idIgnorado);

            long count = (long)(comando.ExecuteScalar() ?? 0);
            return count > 0;
        }
    }

    public static List<Pregunta> ObtenerRosco(SqliteConnection conexion, int temaId)
    {
        List<Pregunta> rosco = new List<Pregunta>();
        string letras = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";

        foreach (char letra in letras)
        {
            Pregunta? pregunta = ObtenerAleatoria(conexion, letra, temaId);
            if (pregunta != null)
            {
                rosco.Add(pregunta);
            }
        }

        return rosco;
    }

    private static Pregunta? ObtenerAleatoria(SqliteConnection conexion, char letra, int temaId)
    {
        string sql = ConsultaBase() + " AND p.letra = @letra ";
        if (temaId > 0)
        {
            sql += " AND p.tema_id = @tema_id ";
        }
        sql += " ORDER BY RANDOM() LIMIT 1";
        using (SqliteCommand comando = new SqliteCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@letra", letra.ToString());
            if (temaId > 0)
            {
                comando.Parameters.AddWithValue("@tema_id", temaId);
            }

            using (SqliteDataReader reader = comando.ExecuteReader())
            {
                if (reader.Read())
                {
                    return CrearDesdeReader(reader);
                }
            }
        }
        return null;
    }

    private static string ConsultaBase()
    {
        return "SELECT p.id AS pregunta_id, p.letra, p.respuesta, p.definicion, "+
               "t.id AS tema_id, t.nombre, t.descripcion " +
               "FROM preguntas p, temas t WHERE p.tema_id = t.id";
    }

    private static Pregunta? CrearDesdeReader(SqliteDataReader reader)
    {
        int id = reader.GetInt32(reader.GetOrdinal("pregunta_id"));
        char letra = reader.GetString(reader.GetOrdinal("letra"))[0];
        string respuesta = reader.GetString(reader.GetOrdinal("respuesta"));
        string definicion = reader.GetString(reader.GetOrdinal("definicion"));

        int temaId = reader.GetInt32(reader.GetOrdinal("tema_id"));
        string temaNombre = reader.GetString(reader.GetOrdinal("nombre"));
        string temaDescripcion = reader.GetString(reader.GetOrdinal("descripcion"));

        Tema tema = new Tema(temaId, temaNombre, temaDescripcion);

        return new Pregunta(id, letra, respuesta, definicion, tema);
    }
}