using Microsoft.AspNetCore.SignalR;

namespace PEPELTORO.Hubs.GatoHubs
{
    public class GatoHub:Hub
    {
        public static Dictionary<string, string> users = new Dictionary<string, string>();

        public async Task IniciarSesion(string nombreUsuario)
        {
            if (users.Keys.Any(
                x => x.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase)
               ))
            {
                await Clients.Caller.SendAsync("Receivemessage", "error", "el nombre de usuario existe");
            }
            else
            {
                //users[Context.ConnectionId]= nombreUsuario;
                users[nombreUsuario] = Context.ConnectionId;
                await Clients.Caller.SendAsync("Receivemessage", "ok", "ya pegaste morro");
            }
        }
        public static Queue<string> cola = new Queue<string>();

        public static int NumPartida = 0;

        public async Task BuscarPartida(string nombreUsuario)
        {
            if (cola.Count == 0)
            {
                cola.Enqueue(nombreUsuario);
            }
            else
            {
                var contrincante = cola.Dequeue();
                string partida = $"partida{NumPartida}";
                await Groups.AddToGroupAsync(Context.ConnectionId, partida);
                await Groups.AddToGroupAsync(users[contrincante], partida);
                NumPartida++;
                await Clients.Groups(partida).SendAsync("Game started", partida);

                //notificar al usuario que es su turno.
                await Clients.Users(Context.ConnectionId).SendAsync("Play");

            }

        }
    }
}
