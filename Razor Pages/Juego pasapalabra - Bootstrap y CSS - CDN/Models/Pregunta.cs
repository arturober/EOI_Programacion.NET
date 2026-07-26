using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.Sqlite;

// Cada objeto Pregunta representa una fila de preguntas y contiene su tema.
public class Pregunta
{
    private static readonly StringComparer OrdenEspanol =
        StringComparer.Create(new CultureInfo("es-ES"), true);

    public int Id { get; set; }

    public char Letra { get; set; }

    [Required(ErrorMessage = "La respuesta es obligatoria.")]
    [StringLength(100, ErrorMessage = "La respuesta no puede superar los 100 caracteres.")]
    public string Respuesta { get; set; } = "";

    [Required(ErrorMessage = "La definición es obligatoria.")]
    [StringLength(500, ErrorMessage = "La definición no puede superar los 500 caracteres.")]
    [Display(Name = "Definición")]
    public string Definicion { get; set; } = "";

    [ValidateNever]
    public Tema Tema { get; set; } = new Tema();

    public Pregunta() { }

    public Pregunta(char letra, string respuesta, string definicion, Tema tema)
    {
        Letra = char.ToUpperInvariant(letra);
        Respuesta = respuesta;
        Definicion = definicion;
        Tema = tema;
    }

    public Pregunta(int id, char letra, string respuesta, string definicion, Tema tema)
        : this(letra, respuesta, definicion, tema)
    {
        Id = id;
    }

    public static void PrepararTabla(SqliteConnection conexion)
    {
        string sql = "CREATE TABLE IF NOT EXISTS preguntas (" +
                     "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                     "letra TEXT NOT NULL, " +
                     "respuesta TEXT NOT NULL, " +
                     "respuesta_normalizada TEXT NOT NULL, " +
                     "definicion TEXT NOT NULL, " +
                     "tema_id INTEGER NOT NULL, " +
                     "UNIQUE (respuesta_normalizada, tema_id), " +
                     "FOREIGN KEY (tema_id) REFERENCES temas(id) ON DELETE RESTRICT)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.ExecuteNonQuery();
    }

    public string ObtenerEnunciado()
    {
        string respuesta = TextoUtil.NormalizarParaComparar(Respuesta);
        char letra = TextoUtil.NormalizarCaracter(Letra);
        string comienzo = respuesta.StartsWith(letra) ? "Empieza por la " : "Contiene la ";
        return comienzo + Letra + ": " + Definicion;
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = """
            INSERT INTO preguntas
            (letra, respuesta, respuesta_normalizada, definicion, tema_id)
            VALUES (@letra, @respuesta, @normalizada, @definicion, @temaId)
            """;
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AnadirParametros(comando);
        int filas = comando.ExecuteNonQuery();
        using SqliteCommand comandoId = new SqliteCommand("SELECT last_insert_rowid()", conexion);
        Id = Convert.ToInt32(comandoId.ExecuteScalar());
        return filas == 1;
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = """
            UPDATE preguntas SET letra = @letra, respuesta = @respuesta,
            respuesta_normalizada = @normalizada, definicion = @definicion,
            tema_id = @temaId WHERE id = @id
            """;
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AnadirParametros(comando);
        comando.Parameters.AddWithValue("@id", Id);
        return comando.ExecuteNonQuery() == 1;
    }

    private void AnadirParametros(SqliteCommand comando)
    {
        comando.Parameters.AddWithValue("@letra", Letra.ToString());
        comando.Parameters.AddWithValue("@respuesta", Respuesta.Trim());
        comando.Parameters.AddWithValue("@normalizada", TextoUtil.NormalizarParaComparar(Respuesta));
        comando.Parameters.AddWithValue("@definicion", Definicion.Trim());
        comando.Parameters.AddWithValue("@temaId", Tema.Id);
    }

    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM preguntas WHERE id = @id";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", Id);
        return comando.ExecuteNonQuery() == 1;
    }

    public static List<Pregunta> Listar(SqliteConnection conexion, string buscar = "")
    {
        List<Pregunta> preguntas = new List<Pregunta>();
        string sql = ConsultaBase();

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        using SqliteDataReader lector = comando.ExecuteReader();

        while (lector.Read())
        {
            preguntas.Add(CrearDesdeLector(lector));
        }

        // El filtrado en C# permite ignorar mayúsculas y tildes.
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            string textoBuscado = TextoUtil.NormalizarParaComparar(buscar);
            preguntas = preguntas
                .Where(pregunta =>
                    TextoUtil.NormalizarParaComparar(pregunta.Respuesta)
                        .Contains(textoBuscado) ||
                    TextoUtil.NormalizarParaComparar(pregunta.Definicion)
                        .Contains(textoBuscado))
                .ToList();
        }

        return preguntas
            .OrderBy(pregunta => pregunta.Tema.Nombre, OrdenEspanol)
            .ThenBy(pregunta => TextoUtil.LetrasRosco.IndexOf(pregunta.Letra))
            .ThenBy(pregunta => pregunta.Respuesta, OrdenEspanol)
            .ToList();
    }

    public static Pregunta? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = ConsultaBase() + " WHERE p.id = @id";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        using SqliteDataReader lector = comando.ExecuteReader();
        return lector.Read() ? CrearDesdeLector(lector) : null;
    }

    public static int ContarPorTema(SqliteConnection conexion, int temaId)
    {
        string sql = "SELECT COUNT(*) FROM preguntas WHERE tema_id = @temaId";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@temaId", temaId);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public static int ContarLetras(SqliteConnection conexion, int temaId)
    {
        string sql = "SELECT COUNT(DISTINCT letra) FROM preguntas";

        if (temaId > 0)
        {
            sql += " WHERE tema_id = @temaId";
        }

        using SqliteCommand comando = new SqliteCommand(sql, conexion);

        if (temaId > 0)
        {
            comando.Parameters.AddWithValue("@temaId", temaId);
        }

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public static List<Pregunta> ObtenerRosco(SqliteConnection conexion, int temaId)
    {
        List<Pregunta> rosco = new List<Pregunta>();
        foreach (char letra in TextoUtil.LetrasRosco)
        {
            string sql = ConsultaBase() + " WHERE p.letra = @letra";

            if (temaId > 0)
            {
                sql += " AND p.tema_id = @temaId";
            }

            sql += " ORDER BY RANDOM() LIMIT 1";

            using SqliteCommand comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@letra", letra.ToString());

            if (temaId > 0)
            {
                comando.Parameters.AddWithValue("@temaId", temaId);
            }

            using SqliteDataReader lector = comando.ExecuteReader();

            if (lector.Read())
            {
                rosco.Add(CrearDesdeLector(lector));
            }
        }
        return rosco;
    }

    private static string ConsultaBase()
    {
        return "SELECT p.id AS pregunta_id, p.letra, p.respuesta, p.definicion, " +
               "t.id AS tema_id, t.nombre, t.descripcion " +
               "FROM preguntas p INNER JOIN temas t ON p.tema_id = t.id";
    }

    private static Pregunta CrearDesdeLector(SqliteDataReader lector)
    {
        Tema tema = new Tema(
            Convert.ToInt32(lector["tema_id"]),
            Convert.ToString(lector["nombre"]) ?? "",
            Convert.ToString(lector["descripcion"]) ?? "");
        return new Pregunta(
            Convert.ToInt32(lector["pregunta_id"]),
            (Convert.ToString(lector["letra"]) ?? "A")[0],
            Convert.ToString(lector["respuesta"]) ?? "",
            Convert.ToString(lector["definicion"]) ?? "",
            tema);
    }

    public static void InsertarDatosIniciales(SqliteConnection conexion)
    {
        string sqlCantidad = "SELECT COUNT(*) FROM temas";
        using SqliteCommand comandoCantidad = new SqliteCommand(sqlCantidad, conexion);
        long cantidad = Convert.ToInt64(comandoCantidad.ExecuteScalar());

        if (cantidad > 0)
        {
            return;
        }

        Tema tema = new Tema(
            "Cultura general", "Un rosco de ejemplo con las 27 letras.");
        tema.Insertar(conexion);

        string[] datos =
        {
            "A|Alfabeto|Conjunto ordenado de letras de una lengua.",
            "B|Biblioteca|Lugar donde se guardan y prestan libros.",
            "C|Calendario|Sistema que organiza los días, meses y años.",
            "D|Diccionario|Libro que explica el significado de las palabras.",
            "E|Eclipse|Ocultación total o parcial de un astro por otro.",
            "F|Fósil|Resto o señal de un ser vivo de épocas pasadas.",
            "G|Galaxia|Conjunto enorme de estrellas, gas y polvo.",
            "H|Hemisferio|Cada una de las dos mitades de una esfera.",
            "I|Isla|Porción de tierra rodeada de agua.",
            "J|Jirafa|Mamífero africano conocido por su largo cuello.",
            "K|Kilómetro|Unidad de longitud equivalente a mil metros.",
            "L|Laberinto|Lugar formado por caminos entrecruzados del que cuesta salir.",
            "M|Microscopio|Instrumento usado para observar objetos muy pequeños.",
            "N|Neptuno|Octavo planeta del sistema solar.",
            "Ñ|España|País europeo cuya capital es Madrid.",
            "O|Océano|Gran extensión de agua salada.",
            "P|Pirámide|Construcción con base poligonal y caras triangulares.",
            "Q|Química|Ciencia que estudia la materia y sus transformaciones.",
            "R|Relámpago|Resplandor producido durante una tormenta.",
            "S|Satélite|Cuerpo que gira alrededor de un planeta.",
            "T|Telescopio|Instrumento para observar objetos muy lejanos.",
            "U|Universo|Conjunto de todo lo que existe.",
            "V|Volcán|Abertura de la corteza terrestre por la que sale magma.",
            "W|Kiwi|Fruta de pulpa verde cuyo nombre contiene una uve doble.",
            "X|Oxígeno|Elemento químico necesario para la respiración.",
            "Y|Yacimiento|Lugar donde se encuentran minerales o restos arqueológicos.",
            "Z|Zoología|Ciencia que estudia los animales."
        };

        foreach (string dato in datos)
        {
            string[] partes = dato.Split('|');
            Pregunta pregunta = new Pregunta(
                partes[0][0], partes[1], partes[2], tema);
            pregunta.Insertar(conexion);
        }
    }
}
