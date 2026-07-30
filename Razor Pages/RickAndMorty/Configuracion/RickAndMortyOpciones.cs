namespace RickAndMorty.Configuracion;

// Contiene los ajustes modificables del cliente de la API.
public class RickAndMortyOpciones
{
    public const string Seccion = "RickAndMortyApi";

    // Los datos de la serie cambian poco, por lo que conviene usar caché.
    public int MinutosCache { get; set; } = 30;
}
