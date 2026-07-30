using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Tierra;

// EONET reúne sucesos naturales y sus coordenadas en formato GeoJSON.
public class EventosModel(
    INasaServicio nasaServicio,
    IFavoritosServicio favoritosServicio,
    UserManager<Usuario> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Estado { get; set; } = "open";

    [BindProperty(SupportsGet = true)]
    public string? Categoria { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Dias { get; set; } = 30;

    public List<EonetEventoDto> Eventos { get; private set; } = [];
    public List<PuntoMapa> Puntos { get; private set; } = [];
    public HashSet<string> Favoritos { get; private set; } = [];
    public string? Error { get; private set; }

    public async Task OnGetAsync()
    {
        Estado = Estado is "open" or "closed" or "all" ? Estado : "open";
        Dias = Math.Clamp(Dias, 1, 365);

        try
        {
            Eventos = await nasaServicio.ObtenerEventosNaturalesAsync(
                Estado,
                Categoria,
                Dias);
            Puntos = Eventos
                .Select(CrearPunto)
                .Where(punto => punto is not null)
                .Cast<PuntoMapa>()
                .ToList();

            if (User.Identity?.IsAuthenticated == true)
            {
                string usuarioId = userManager.GetUserId(User)!;
                Favoritos = await favoritosServicio.ObtenerReferenciasAsync(
                    usuarioId,
                    "EONET");
            }
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }
    }

    private static PuntoMapa? CrearPunto(EonetEventoDto evento)
    {
        // La geometría más reciente suele representar mejor la posición actual.
        EonetGeometriaDto? geometria = evento.Geometrias
            .OrderByDescending(item => item.Fecha)
            .FirstOrDefault(item =>
                item.Tipo == "Point"
                && item.Coordenadas.ValueKind == JsonValueKind.Array
                && item.Coordenadas.GetArrayLength() >= 2);

        if (geometria is null)
        {
            return null;
        }

        return new PuntoMapa
        {
            Id = evento.Id,
            Titulo = evento.Titulo,
            Categoria = evento.Categorias.FirstOrDefault()?.Titulo ?? "Evento",
            Longitud = geometria.Coordenadas[0].GetDouble(),
            Latitud = geometria.Coordenadas[1].GetDouble(),
            Fecha = geometria.Fecha.ToString("dd/MM/yyyy")
        };
    }

    // Se serializa como JSON para crear marcadores con Leaflet.
    public class PuntoMapa
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Fecha { get; set; } = string.Empty;
    }
}
