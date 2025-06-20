﻿using MusicPlayer.Extensions;
using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The video controller class.
    /// </summary>
    internal class VideoController : MusicPlayerWrapper, IVideo
    {
        /// <summary>
        /// The music player.
        /// </summary>
        private new readonly IMusicPlayer _player;

        /// <summary>
        /// The thread that will send ocasional messages.
        /// </summary>
        private Thread _refreshClientInfo;

        /// <summary>
        /// The current video url.
        /// </summary>
        private string _videoUrl;

        /// <summary>
        /// The reference of the video.
        /// </summary>
        private DateTime _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoController" /> class. 
        /// </summary>
        /// <param name="player">The music player.</param>
        public VideoController(IMusicPlayer player) : base(player)
        {
            _player = player;
        }

        /// <summary>
        /// Start a video.
        /// </summary>
        /// <param name="url">The url of the video.</param>
        public void StartVideo(string url)
        {
            _player?.TogglePlay(true);
            var server = _player as IServer;
            _videoUrl = url;

            _refreshClientInfo?.Abort();
            _refreshClientInfo = new Thread(CheckTime);
            _refreshClientInfo.Start();

            if (server != null)
            {
                server.GetInfo().VideoUrl = url;
                server.TogglePlay(true);
                server.SetVideo(url);
            }
        }

        /// <summary>
        /// Seek to a specific position.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        public void Seek(double position)
        {
            var server = _player as IServer;
            _started = DateTime.Now - TimeSpan.FromSeconds(position);
            server?.SeekVideo(position);
        }

        /// <summary>
        /// Gets the youtube channels video's.
        /// </summary>
        /// <param name="id">The id of the youtube channel.</param>
        /// <returns>A list of video info.</returns>
        public async Task<List<VideoInfo>> GetYoutubeChannel(string id)
        {
            var client = new YoutubeClient();
            var videos = await client.GetChannelUploadsAsync(id);
            return videos.Select(v => new VideoInfo(v)).ToList();
        }

        /// <summary>
        /// Gets the playlists videos.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <returns>The video info.</returns>
        public async Task<List<VideoInfo>> GetYoutubePlayList(string id)
        {
            var client = new YoutubeClient();
            var playlist = await client.GetPlaylistAsync(id);
            return playlist.Videos.Select(v => new VideoInfo(v)).ToList();
        }

        /// <summary>
        /// Stop the video.
        /// </summary>
        /// <returns>The music player.</returns>
        public IMusicPlayer StopVideo()
        {
            var server = _player as IServer;
            _videoUrl = null;

            if (server != null)
            {
                server.GetInfo().VideoUrl = null;
                server.SetVideo(string.Empty);
            }

            return _player;
        }

        /// <summary>
        /// Dispose of the controller.
        /// </summary>
        public override void Dispose()
        {
            _videoUrl = null;
            _refreshClientInfo?.Abort();
            base.Dispose();
        }

        /// <summary>
        /// This thread sends the video and time to the clients every 5 seconds.
        /// </summary>
        private void CheckTime()
        {
            _started = DateTime.Now;

            while (!string.IsNullOrEmpty(_videoUrl))
            {
                if (_player is IServer server)
                {
                    server.SetVideo(_videoUrl);
                    var elapsed = DateTime.Now - _started;
                    server.SeekVideo(elapsed.TotalSeconds);
                }

                ThreadExtensions.SaveSleep(5000);
            }
        }
    }
}
