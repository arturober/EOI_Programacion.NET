using MazmorraOnline.Models;
using MazmorraOnline.Services;
using Microsoft.AspNetCore.SignalR;

namespace MazmorraOnline.Hubs;

// Recibe las acciones de los navegadores y envía el estado en tiempo real.
public class JuegoHub : Hub
{
    private readonly GestorJuego _gestorJuego;

    public JuegoHub(GestorJuego gestorJuego)
    {
        _gestorJuego = gestorJuego;
    }

    public async Task<bool> EntrarEnPartida(string jugadorId)
    {
        // Una reconexión sustituye la conexión anterior del mismo jugador.
        bool conectado = _gestorJuego.ConectarJugador(
            jugadorId,
            Context.ConnectionId);

        if (!conectado)
        {
            // Devolver false evita registrar una excepción de SignalR por una
            // sesión que simplemente ha caducado.
            return false;
        }

        // Context.Items permite recordar qué jugador utiliza esta conexión.
        Context.Items["jugadorId"] = jugadorId;

        // Al entrar se envía inmediatamente el estado, sin esperar al motor.
        await Clients.Caller.SendAsync(
            "EstadoActualizado",
            _gestorJuego.ObtenerEstado());

        return true;
    }

    public void EnviarAccion(AccionJugador accion)
    {
        // Se recupera el jugador asociado a esta conexión de SignalR.
        Context.Items.TryGetValue("jugadorId", out object? valorJugadorId);

        string? jugadorId = valorJugadorId as string;

        if (jugadorId is not null)
        {
            _gestorJuego.GuardarAccion(
                jugadorId,
                Context.ConnectionId,
                accion);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // La desconexión puede ser temporal, por ejemplo al cambiar de red.
        Context.Items.TryGetValue("jugadorId", out object? valorJugadorId);

        string? jugadorId = valorJugadorId as string;

        if (jugadorId is not null)
        {
            _gestorJuego.MarcarJugadorDesconectado(
                jugadorId,
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
