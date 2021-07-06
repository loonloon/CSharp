using MusicPlayer.Interface;
using System;
using MusicPlayer.Models;
using System.Net;
using System.ServiceModel;
using System.IO;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// A music player client.
    /// </summary>
    internal class ClientConnection : MusicPlayerWrapper, IClient
    {
        /// <summary>
        /// The server contract.
        /// </summary>
        private readonly IServerContract _server;

        /// <summary>
        /// The WCF client.
        /// </summary>
        private readonly WCFServerClient _client;

        /// <summary>
        /// The channel factory.
        /// </summary>
        private readonly DuplexChannelFactory<IServerContract> _factory;

        /// <summary>
        /// The server info.
        /// </summary>
        private ServerInfo _serverInfo;

        /// <summary>
        /// Server info changed.
        /// </summary>
        public event ServerInfoChanged OnInfoChanged;

        /// <summary>
        /// Create a connection to a musicplayer server.
        /// </summary>
        /// <param name="player">The musicplayer.</param>
        /// <param name="address">The server ip address.</param>
        /// <param name="port">The server port.</param>
        public ClientConnection(IMusicPlayer player, IPAddress address, int port) : base(player)
        {
            _client = new WCFServerClient(this);
            var serviceAddress = $"net.tcp://{address}:{port}/";
            var binding = new NetTcpContextBinding(SecurityMode.None)
            {
                CloseTimeout = new TimeSpan(0, 1, 0),
                ReceiveTimeout = new TimeSpan(1, 0, 0),
                SendTimeout = new TimeSpan(0, 1, 0),
                MaxReceivedMessageSize = 100000000
            };
            // 100Mb
            _factory = new DuplexChannelFactory<IServerContract>(new InstanceContext(_client), binding, serviceAddress);
            _server = _factory.CreateChannel();
            _server.Anounce();
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        /// <returns>The musicplayer.</returns>
        public IMusicPlayer Disconnect()
        {
            if (_factory.State == CommunicationState.Opened)
            {
                try
                {
                    _server.Goodbye();
                    _factory.Close();
                }
                catch
                {
                    // ignored
                }
            }

            _factory.Abort();
            OnInfoChanged?.Invoke(null);
            base.Dispose();
            return null;
        }

        /// <summary>
        /// Gets the server info.
        /// </summary>
        /// <returns></returns>
        public ServerInfo GetInfo()
        {
            return _serverInfo ?? (_serverInfo = new ServerInfo
            {
                IsHost = false,
                Host = _factory.Endpoint.Address.Uri.DnsSafeHost,
                Clients = null,
                Port = _factory.Endpoint.Address.Uri.Port
            });
        }

        /// <summary>
        /// Sets the video.
        /// </summary>
        /// <param name="url"></param>
        public void SetVideo(string url)
        {
            var info = GetInfo();
            info.VideoUrl = url;
            OnInfoChanged?.Invoke(info);
        }

        /// <summary>
        /// Play the song.
        /// </summary>
        /// <param name="song">The song.</param>
        public override void Play(SongInformation song)
        {
            if (!song.IsInternetLocation)
            {
                var filePath =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\\Music player downloaded files\\{song.FileName}";
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                song.Location = filePath;
                song.DateAdded = DateTime.Now;

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileStream.Write(song.File, 0, song.File.Length);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Could not write file");
                }
            }

            song.File = null;
            base.Play(song);
            var position = _server.GetCurrentPosition();

            if (position > 0)
            {
                base.MoveToTime(Convert.ToInt64(position));
            }
        }

        /// <summary>
        /// Seek to a position in  the video.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        public void SeekVideo(double position)
        {
            var info = GetInfo();
            info.VideoPosition = position;
            OnInfoChanged?.Invoke(info);
        }

        /// <summary>
        /// Disconnect in a clean way.
        /// </summary>
        public override void Dispose()
        {
            Disconnect();
            base.Dispose();
        }
    }
}
