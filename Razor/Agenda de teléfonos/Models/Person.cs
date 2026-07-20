using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;

namespace AgendaContactosWeb.Models;

// Cada objeto Persona representa una fila de la tabla personas.
public class Persona
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(30, ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = "";

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO personas (nombre, telefono) VALUES (@nombre, @telefono)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
        comando.Parameters.AddWithValue("@telefono", Telefono.Trim());

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE personas SET nombre = @nombre, telefono = @telefono WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
        comando.Parameters.AddWithValue("@telefono", Telefono.Trim());
        comando.Parameters.AddWithValue("@id", Id);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Borrar(SqliteConnection conexion)
    {
        string sql = "DELETE FROM personas WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", Id);

        return comando.ExecuteNonQuery() > 0;
    }

    public static List<Persona> Listar(SqliteConnection conexion, string busqueda = "")
    {
        List<Persona> personas = new List<Persona>();
        string sql = "SELECT id, nombre, telefono FROM personas " +
                     "WHERE nombre LIKE @busqueda ORDER BY nombre";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@busqueda", "%" + busqueda.Trim() + "%");

        using SqliteDataReader lector = comando.ExecuteReader();
        while (lector.Read())
        {
            personas.Add(CrearDesdeLector(lector));
        }

        return personas;
    }

    public static Persona? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT id, nombre, telefono FROM personas WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);

        using SqliteDataReader lector = comando.ExecuteReader();
        return lector.Read() ? CrearDesdeLector(lector) : null;
    }

    private static Persona CrearDesdeLector(SqliteDataReader lector)
    {
        return new Persona
        {
            Id = Convert.ToInt32(lector["id"]),
            Nombre = Convert.ToString(lector["nombre"]) ?? "",
            Telefono = Convert.ToString(lector["telefono"]) ?? ""
        };
    }
}
