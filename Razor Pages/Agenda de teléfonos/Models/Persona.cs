using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.Sqlite;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace AgendaTelefonos.Models;

// Cada objeto Persona representa una fila de la tabla personas.
public class Persona
{
    private const long TamanoMaximoImagen = 2 * 1024 * 1024;
    private const int AnchoMaximoImagen = 48;

    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(30, ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = "";

    // La imagen se guarda como texto Base64 dentro de SQLite.
    [BindNever]
    public string? ImagenBase64 { get; set; }

    public static void PrepararTabla(SqliteConnection conexion)
    {
        string sql = "CREATE TABLE IF NOT EXISTS personas (" +
                     "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                     "nombre TEXT NOT NULL, " +
                     "telefono TEXT NOT NULL, " +
                     "imagen TEXT)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.ExecuteNonQuery();

        // Permitimos abrir también bases de datos creadas con versiones antiguas.
        AgregarColumnaSiNoExiste(conexion, "imagen", "TEXT");
    }

    public bool Insertar(SqliteConnection conexion)
    {
        string sql = "INSERT INTO personas (nombre, telefono, imagen) " +
                     "VALUES (@nombre, @telefono, @imagen)";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando);

        return comando.ExecuteNonQuery() > 0;
    }

    public bool Actualizar(SqliteConnection conexion)
    {
        string sql = "UPDATE personas SET nombre = @nombre, telefono = @telefono, " +
                     "imagen = @imagen WHERE id = @id";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        AgregarParametros(comando);
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

    private void AgregarParametros(SqliteCommand comando)
    {
        comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
        comando.Parameters.AddWithValue("@telefono", Telefono.Trim());
        comando.Parameters.AddWithValue("@imagen", (object?)ImagenBase64 ?? DBNull.Value);
    }

    public static List<Persona> Listar(
        SqliteConnection conexion, string? busqueda = null)
    {
        List<Persona> personas = new List<Persona>();
        string sql = "SELECT id, nombre, telefono, imagen FROM personas";

        using SqliteCommand comando = new SqliteCommand(sql, conexion);

        using SqliteDataReader lector = comando.ExecuteReader();
        while (lector.Read())
        {
            personas.Add(CrearDesdeLector(lector));
        }

        // El filtrado se hace en C# para poder ignorar mayúsculas y tildes.
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string textoBuscado = Normalizar(busqueda.Trim());
            personas = personas
                .Where(persona => Normalizar(persona.Nombre).Contains(textoBuscado))
                .ToList();
        }

        return personas
            .OrderBy(persona => persona.Nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    public static Persona? BuscarPorId(SqliteConnection conexion, int id)
    {
        string sql = "SELECT id, nombre, telefono, imagen " +
                     "FROM personas WHERE id = @id";

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
            Telefono = Convert.ToString(lector["telefono"]) ?? "",
            ImagenBase64 = lector["imagen"] == DBNull.Value
                ? null
                : Convert.ToString(lector["imagen"])
        };
    }

    public string? ProcesarImagen(IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return null;
        }

        if (archivo.Length > TamanoMaximoImagen)
        {
            return "La imagen no puede superar los 2 MB.";
        }

        try
        {
            using Image imagen = Image.Load(archivo.OpenReadStream());
            string formato = imagen.Metadata.DecodedImageFormat?.Name.ToUpperInvariant() ?? "";

            if (formato != "JPEG" && formato != "PNG" &&
                formato != "WEBP" && formato != "GIF")
            {
                return "La imagen debe estar en formato JPG, PNG, WEBP o GIF.";
            }

            imagen.Mutate(x => x.AutoOrient());

            if (imagen.Width > AnchoMaximoImagen)
            {
                imagen.Mutate(x => x.Resize(AnchoMaximoImagen, 0));
            }

            using MemoryStream memoria = new MemoryStream();
            imagen.SaveAsPng(memoria);
            ImagenBase64 = Convert.ToBase64String(memoria.ToArray());

            return null;
        }
        catch (ImageFormatException)
        {
            return "El archivo seleccionado no contiene una imagen válida.";
        }
    }

    public string? ObtenerImagenComoDataUrl()
    {
        if (string.IsNullOrWhiteSpace(ImagenBase64))
        {
            return null;
        }

        return $"data:image/png;base64,{ImagenBase64}";
    }

    private static void AgregarColumnaSiNoExiste(
        SqliteConnection conexion, string nombre, string tipo)
    {
        using SqliteCommand consulta = new SqliteCommand(
            "PRAGMA table_info(personas)", conexion);
        using SqliteDataReader lector = consulta.ExecuteReader();

        while (lector.Read())
        {
            if (Convert.ToString(lector["name"]) == nombre)
            {
                return;
            }
        }

        lector.Close();

        // Estos valores son constantes del programa, no datos del usuario.
        string sql = $"ALTER TABLE personas ADD COLUMN {nombre} {tipo}";
        using SqliteCommand comando = new SqliteCommand(sql, conexion);
        comando.ExecuteNonQuery();
    }

    private static string Normalizar(string texto)
    {
        StringBuilder resultado = new StringBuilder();

        foreach (char caracter in texto.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) !=
                UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(char.ToLowerInvariant(caracter));
            }
        }

        return resultado.ToString();
    }
}
