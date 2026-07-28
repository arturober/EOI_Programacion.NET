namespace Pokemon.Models;

// Representa un Pokémon en las tarjetas de la página principal.
public class PokemonResumen
{
    // Número identificador de PokeAPI.
    public int Id { get; set; }

    // Nombre utilizado por PokeAPI.
    public string Nombre { get; set; } = "";

    // Dirección de la ilustración oficial.
    public string Imagen { get; set; } = "";

    // Devuelve el nombre preparado para mostrarlo en pantalla.
    public string NombreVisible => TextoPokemon.Formatear(Nombre);
}

// Contiene una página de resultados y los datos necesarios para paginar.
public class ResultadoPokemon
{
    // Pokémon que se mostrarán en la página actual.
    public List<PokemonResumen> Pokemons { get; set; } = new();

    // Número de página actual.
    public int Pagina { get; set; }

    // Número total de páginas.
    public int TotalPaginas { get; set; }

    // Número total de Pokémon encontrados.
    public int TotalResultados { get; set; }
}

// Representa una de las imágenes encontradas dentro de "sprites".
public class ImagenPokemon
{
    // Texto que explica de qué juego o apartado procede la imagen.
    public string Descripcion { get; set; } = "";

    // Dirección completa de la imagen.
    public string Url { get; set; } = "";
}

// Representa una habilidad del Pokémon.
public class HabilidadPokemon
{
    // Nombre de la habilidad.
    public string Nombre { get; set; } = "";

    // Indica si se trata de una habilidad oculta.
    public bool EsOculta { get; set; }
}

// Representa una estadística base, por ejemplo ataque o velocidad.
public class EstadisticaPokemon
{
    // Nombre traducido de la estadística.
    public string Nombre { get; set; } = "";

    // Valor base de la estadística.
    public int Valor { get; set; }

    // Puntos de esfuerzo que aporta.
    public int Esfuerzo { get; set; }
}

// Representa un objeto que puede llevar el Pokémon.
public class ObjetoPokemon
{
    // Nombre del objeto.
    public string Nombre { get; set; } = "";

    // Versiones del juego en las que puede llevarlo.
    public List<string> Versiones { get; set; } = new();
}

// Representa un lugar en el que se puede encontrar al Pokémon.
public class EncuentroPokemon
{
    // Nombre de la zona.
    public string Zona { get; set; } = "";

    // Probabilidad máxima indicada por PokeAPI.
    public int ProbabilidadMaxima { get; set; }
}

// Reúne toda la información que utiliza la página de detalle.
public class PokemonDetalle
{
    // Datos básicos del recurso pokemon.
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string NombreEspanol { get; set; } = "";
    public int? ExperienciaBase { get; set; }
    public decimal AlturaMetros { get; set; }
    public decimal PesoKilos { get; set; }
    public int Orden { get; set; }
    public bool EsVariedadPrincipal { get; set; }

    // Datos procedentes del recurso pokemon-species.
    public string Genero { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Generacion { get; set; } = "";
    public string Habitat { get; set; } = "";
    public string Color { get; set; } = "";
    public string Forma { get; set; } = "";
    public string Crecimiento { get; set; } = "";
    public int RatioCaptura { get; set; }
    public int FelicidadBase { get; set; }
    public int PasosEclosionAproximados { get; set; }
    public string DistribucionSexo { get; set; } = "";
    public bool EsBebe { get; set; }
    public bool EsLegendario { get; set; }
    public bool EsMitico { get; set; }
    public bool TieneDiferenciasDeSexo { get; set; }

    // Colecciones mostradas en las diferentes secciones.
    public List<string> Tipos { get; set; } = new();
    public List<HabilidadPokemon> Habilidades { get; set; } = new();
    public List<EstadisticaPokemon> Estadisticas { get; set; } = new();
    public List<string> Movimientos { get; set; } = new();
    public List<string> Formas { get; set; } = new();
    public List<string> Versiones { get; set; } = new();
    public List<string> GruposHuevo { get; set; } = new();
    public List<string> Variedades { get; set; } = new();
    public List<string> CadenaEvolucion { get; set; } = new();
    public List<ObjetoPokemon> Objetos { get; set; } = new();
    public List<EncuentroPokemon> Encuentros { get; set; } = new();
    public List<ImagenPokemon> Imagenes { get; set; } = new();

    // Direcciones de los dos sonidos que ofrece actualmente PokeAPI.
    public string? SonidoActual { get; set; }
    public string? SonidoClasico { get; set; }

    // JSON completo para comprobar campos que no se muestran de forma visual.
    public string JsonOriginal { get; set; } = "";
}

// Esta clase estática contiene pequeñas funciones comunes de presentación.
public static class TextoPokemon
{
    // Convierte "mr-mime" en "Mr Mime".
    public static string Formatear(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "No disponible";
        }

        string textoConEspacios = texto.Replace("-", " ").Replace("_", " ");
        return char.ToUpper(textoConEspacios[0]) + textoConEspacios[1..];
    }
}
