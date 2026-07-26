using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Models;

// Cada objeto Categoria representa una fila de la tabla categorias.
public class Categoria
{
    private static readonly StringComparer OrdenEspanol =
        StringComparer.Create(new CultureInfo("es-ES"), true);

    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = "";

    public static void PrepararTabla(SqliteConnection conexion)
    {
        string sql = "CREATE TABLE IF NOT EXISTS categorias (" +
                     "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                     "nombre TEXT NOT NULL COLLATE NOCASE UNIQUE, " +
                     "descripcion TEXT NOT NULL)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.ExecuteNonQuery();
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO categorias (nombre, descripcion) VALUES (@nombre, @descripcion)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE categorias SET nombre = @nombre, " +
                     "descripcion = @descripcion WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando);
        comando.Parameters.AddWithValue("@id", Id);

        return comando.ExecuteNonQuery() > 0;
    }

    private void AgregarParametros(SqliteCommand comando)
    {
        comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
        comando.Parameters.AddWithValue("@descripcion", Descripcion.Trim());
    }

    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM categorias WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", Id);

        return comando.ExecuteNonQuery() > 0;
    }

    public int ContarTareas(SqliteConnection conexion)
    {
        string sql = "SELECT COUNT(*) FROM tareas WHERE categoria_id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", Id);

        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public static bool ExisteNombre(
        SqliteConnection conexion, string nombre, int idIgnorado = 0)
    {
        string sql = "SELECT COUNT(*) FROM categorias " +
                     "WHERE nombre = @nombre COLLATE NOCASE AND id <> @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", nombre.Trim());
        comando.Parameters.AddWithValue("@id", idIgnorado);

        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    public static List<Categoria> Listar(SqliteConnection conexion)
    {
        List<Categoria> categorias = new List<Categoria>();
        string sql = "SELECT id, nombre, descripcion FROM categorias";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        using SqliteDataReader lector = comando.ExecuteReader();

        while (lector.Read())
        {
            categorias.Add(CrearDesdeLector(lector));
        }

        // La cultura española coloca correctamente Á con A y Ñ después de N.
        return categorias
            .OrderBy(categoria => categoria.Nombre, OrdenEspanol)
            .ToList();
    }

    public static Categoria? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT id, nombre, descripcion FROM categorias WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        using SqliteDataReader lector = comando.ExecuteReader();

        return lector.Read() ? CrearDesdeLector(lector) : null;
    }

    private static Categoria CrearDesdeLector(SqliteDataReader lector)
    {
        return new Categoria
        {
            Id = Convert.ToInt32(lector["id"]),
            Nombre = Convert.ToString(lector["nombre"]) ?? "",
            Descripcion = Convert.ToString(lector["descripcion"]) ?? ""
        };
    }
}
