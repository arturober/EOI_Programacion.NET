using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Data.Sqlite;

// Cada objeto Tema representa una fila de la tabla temas.
public class Tema
{
    private static readonly StringComparer OrdenEspanol =
        StringComparer.Create(new CultureInfo("es-ES"), true);

    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = "";

    public Tema() { }

    public Tema(string nombre, string descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public Tema(int id, string nombre, string descripcion)
    {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public static void PrepararTabla(SqliteConnection conexion)
    {
        string sql = "CREATE TABLE IF NOT EXISTS temas (" +
                     "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                     "nombre TEXT NOT NULL UNIQUE COLLATE NOCASE, " +
                     "descripcion TEXT NOT NULL)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.ExecuteNonQuery();
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO temas (nombre, descripcion) VALUES (@nombre, @descripcion)";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
        comando.Parameters.AddWithValue("@descripcion", Descripcion.Trim());
        int filas = comando.ExecuteNonQuery();
        using SqliteCommand comandoId = new SqliteCommand("SELECT last_insert_rowid()", conexion);
        Id = Convert.ToInt32(comandoId.ExecuteScalar());
        return filas == 1;
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE temas SET nombre = @nombre, descripcion = @descripcion WHERE id = @id";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
        comando.Parameters.AddWithValue("@descripcion", Descripcion.Trim());
        comando.Parameters.AddWithValue("@id", Id);
        return comando.ExecuteNonQuery() == 1;
    }

    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM temas WHERE id = @id";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", Id);
        return comando.ExecuteNonQuery() == 1;
    }

    public static List<Tema> Listar(SqliteConnection conexion)
    {
        List<Tema> temas = new List<Tema>();
        string sql = "SELECT id, nombre, descripcion FROM temas";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        using SqliteDataReader lector = comando.ExecuteReader();

        while (lector.Read())
        {
            temas.Add(CrearDesdeLector(lector));
        }
        // La cultura española coloca Á con A y Ñ después de N.
        return temas
            .OrderBy(tema => tema.Nombre, OrdenEspanol)
            .ToList();
    }

    public static Tema? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT id, nombre, descripcion FROM temas WHERE id = @id";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        using SqliteDataReader lector = comando.ExecuteReader();
        return lector.Read() ? CrearDesdeLector(lector) : null;
    }

    private static Tema CrearDesdeLector(SqliteDataReader lector)
    {
        return new Tema(
            Convert.ToInt32(lector["id"]),
            Convert.ToString(lector["nombre"]) ?? "",
            Convert.ToString(lector["descripcion"]) ?? "");
    }
}
