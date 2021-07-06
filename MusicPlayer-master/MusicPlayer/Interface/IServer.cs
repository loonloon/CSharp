using MusicPlayer.Models;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// Describes a server info change.
    /// </summary>
    public delegate void ServerInfoChanged(ServerInfo serverInfo);

    /// <summary>
    /// The interface describing a music server.
    /// </summary>
    public interface IServer : IMusicPlayer, INetwork
    {
    }
}
