using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.Sqlite;

namespace ListaTareas.Models;

// Cada objeto Tarea representa una fila de tareas y contiene su categoría.
public class Tarea
{
    private static readonly StringComparer OrdenEspanol =
        StringComparer.Create(new CultureInfo("es-ES"), true);

    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = "";

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = "";

    public bool Completada { get; set; }

    [ValidateNever]
    public Categoria Categoria { get; set; } = new Categoria();

    public static void PrepararTabla(SqliteConnection conexion)
    {
        string sql = "CREATE TABLE IF NOT EXISTS tareas (" +
                     "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                     "titulo TEXT NOT NULL, " +
                     "descripcion TEXT NOT NULL, " +
                     "completada INTEGER NOT NULL DEFAULT 0, " +
                     "categoria_id INTEGER NOT NULL, " +
                     "FOREIGN KEY (categoria_id) REFERENCES categorias(id) " +
                     "ON DELETE RESTRICT)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.ExecuteNonQuery();
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO tareas (titulo, descripcion, completada, categoria_id) " +
                     "VALUES (@titulo, @descripcion, @completada, @categoriaId)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE tareas SET titulo = @titulo, descripcion = @descripcion, " +
                     "completada = @completada, categoria_id = @categoriaId WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando);
        comando.Parameters.AddWithValue("@id", Id);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM tareas WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", Id);

        return comando.ExecuteNonQuery() > 0;
    }

    private void AgregarParametros(SqliteCommand comando)
    {
        comando.Parameters.AddWithValue("@titulo", Titulo.Trim());
        comando.Parameters.AddWithValue("@descripcion", Descripcion.Trim());
        comando.Parameters.AddWithValue("@completada", Completada ? 1 : 0);
        comando.Parameters.AddWithValue("@categoriaId", Categoria.Id);
    }

    public static List<Tarea> Listar(SqliteConnection conexion, int? categoriaId = null)
    {
        List<Tarea> tareas = new List<Tarea>();

        string sql = "SELECT t.id AS tarea_id, t.titulo, t.descripcion, t.completada, " +
                     "c.id AS categoria_id, c.nombre AS categoria_nombre, " +
                     "c.descripcion AS categoria_descripcion " +
                     "FROM tareas t INNER JOIN categorias c ON t.categoria_id = c.id ";

        if (categoriaId != null)
        {
            sql += "WHERE c.id = @categoriaId ";
        }

        using SqliteCommand comando = new SqliteCommand(sql, conexion);

        if (categoriaId != null)
        {
            comando.Parameters.AddWithValue("@categoriaId", categoriaId);
        }

        using SqliteDataReader lector = comando.ExecuteReader();

        while (lector.Read())
        {
            tareas.Add(CrearDesdeLector(lector));
        }

        // Primero se muestran las pendientes y después se ordenan por título.
        return tareas
            .OrderBy(tarea => tarea.Completada)
            .ThenBy(tarea => tarea.Titulo, OrdenEspanol)
            .ToList();
    }

    public static Tarea? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT t.id AS tarea_id, t.titulo, t.descripcion, t.completada, " +
                     "c.id AS categoria_id, c.nombre AS categoria_nombre, " +
                     "c.descripcion AS categoria_descripcion " +
                     "FROM tareas t INNER JOIN categorias c ON t.categoria_id = c.id " +
                     "WHERE t.id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        using SqliteDataReader lector = comando.ExecuteReader();

        return lector.Read() ? CrearDesdeLector(lector) : null;
    }

    private static Tarea CrearDesdeLector(SqliteDataReader lector)
    {
        Categoria categoria = new Categoria
        {
            Id = Convert.ToInt32(lector["categoria_id"]),
            Nombre = Convert.ToString(lector["categoria_nombre"]) ?? "",
            Descripcion = Convert.ToString(lector["categoria_descripcion"]) ?? ""
        };

        return new Tarea
        {
            Id = Convert.ToInt32(lector["tarea_id"]),
            Titulo = Convert.ToString(lector["titulo"]) ?? "",
            Descripcion = Convert.ToString(lector["descripcion"]) ?? "",
            Completada = Convert.ToBoolean(lector["completada"]),
            Categoria = categoria
        };
    }
}
