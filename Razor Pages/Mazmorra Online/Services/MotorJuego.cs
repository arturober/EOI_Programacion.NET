using MazmorraOnline.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MazmorraOnline.Services;

// Ejecuta el bucle del juego en segundo plano mientras funciona el servidor.
public class MotorJuego : BackgroundService
{
    private readonly GestorJuego _gestorJuego;
    private readonly IHubContext<JuegoHub> _hubContext;

    public MotorJuego(
        GestorJuego gestorJuego,
        IHubContext<JuegoHub> hubContext)
    {
        _gestorJuego = gestorJuego;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        // Cien milisegundos equivalen a diez actualizaciones por segundo.
        using PeriodicTimer temporizador =
            new(TimeSpan.FromMilliseconds(100));

        while (await temporizador.WaitForNextTickAsync(stoppingToken))
        {
            // Primero se calcula la física de la siguiente décima de segundo.
            _gestorJuego.Actualizar(0.1f);

            // Después se envía el nuevo estado a todos los navegadores.
            await _hubContext.Clients.All.SendAsync(
                "EstadoActualizado",
                _gestorJuego.ObtenerEstado(),
                stoppingToken);
        }
    }
}
