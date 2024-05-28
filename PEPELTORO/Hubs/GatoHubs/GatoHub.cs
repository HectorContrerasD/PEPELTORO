using Microsoft.AspNetCore.SignalR;
using PEPELTORO.Models;

namespace PEPELTORO.Hubs.GatoHubs
{
    public class GatoHub:Hub
    {
        public static Dictionary<string, string> users = new Dictionary<string, string>();
        public static Dictionary<string, Partida> partidas = new Dictionary<string, Partida>();

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
                var datospartida = new Partida()
                {
                    NombrePartida = partida,
                    NombreUsuario1 = nombreUsuario,
                    ConnectionId1 = Context.ConnectionId,
                    NombreUsuario2 = contrincante,
                    ConnectionId2 = users[contrincante],
                    Turno = 'X'
                };

                partidas[partida] = datospartida;

            }

        }


        public async Task Jugar(string partida, string nombreUsuario, string tablero)
        {
            var datosPartida = partidas[partida];
            if (GanaXO(datosPartida.Turno, tablero))
            {
                //Se notifica que gano
                await Clients.Group(partida).SendAsync("GameOver", nombreUsuario);
            }
            else
            {
                datosPartida.Turno = (datosPartida.Turno == 'X' ? 'O' : 'X');
                await Clients.Group(partida).SendAsync(""); //pendiente
            }
        }

        private bool GanaXO(char turno, string tablero)
        {
            return false;
        }

        int[,] lineas = new int[,]
        {
            {0,1,2 },   //...........
            {3,4,5 },
            {6,7,8 },
            {0,3,6 },
            {1,4,7 },
            {2,5,8 }
        };

        //AVANZAR A CLIENTE.
    }
}
