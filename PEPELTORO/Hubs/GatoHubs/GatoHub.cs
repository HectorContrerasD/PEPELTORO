using Microsoft.AspNetCore.SignalR;

namespace PEPELTORO.Hubs.GatoHubs
{
    public class GatoHub:Hub
    {
        public static Dictionary<string, string> users= new Dictionary<string, string>();

        public async Task IniciarSesion(string nombreUsuario)
        {
            if (users.Values.Any(
                x=>x.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase)
               ))
            {
                await Clients.Caller.SendAsync("Receivemessage", "error", "el nombre de usuario existe");
            }
            else
            {
                users[Context.ConnectionId]= nombreUsuario;
                await Clients.Caller.SendAsync("Receivemessage", "ok", "ya pegaste morro");
            }
        }
        public static Queue<string> cola = new Queue<string>();
        public static  Task BuscarPartida(string nombreUsuario)
        {
            return null;
        } /////pll
    }
}
