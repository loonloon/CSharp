using MusicPlayer.Interface;
using MusicPlayer.Models;
using System.Collections.Generic;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The adapter class for the music player. 
    /// </summary>
    internal class MusicPlayerWrapper : IMusicPlayer
    {
        /// <summary>
        /// The real music player.
        /// </summary>
        protected IMusicPlayer Player;

        /// <summary>
        /// The song changed event.
        /// </summary>
        public event SongChanged SongChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicPlayerWrapper" /> class.
        /// </summary>
        /// <param name="player">The music player.</param>
        public MusicPlayerWrapper(IMusicPlayer player)
        {
            Player = player;
            Player.SongChanged += InvokeSongChanged;
        }

        /// <summary>
        /// Invoke the songChanged event.
        /// </summary>
        /// <param name="song">The song.</param>
        public void InvokeSongChanged(SongInformation song)
        {
            SongChanged?.Invoke(song);
        }

        public virtual List<SongInformation> LoadFolder(string folder)
        {
            return Player.LoadFolder(folder);
        }

        public virtual SongInformation GetCurrentSong()
        {
            return Player.GetCurrentSong();
        }

        public virtual List<SongInformation> GetSongs(int index = 0, string query = null, int amount = 50)
        {
            return Player.GetSongs(index, query, amount);
        }

        public virtual void Play(SongInformation song)
        {
            Player.Play(song);
        }

        public virtual bool TogglePlay(bool? pause)
        {
            return Player.TogglePlay(pause);
        }

        public virtual void Next()
        {
            Player.Next();
        }

        public virtual void MoveToTime(long seconds)
        {
            Player.MoveToTime(seconds);
        }

        public virtual bool GetShuffle()
        {
            return Player.GetShuffle();
        }

        public virtual void SetShuffle(bool shuffle)
        {
            Player.SetShuffle(shuffle);
        }

        public virtual int GetVolume()
        {
            return Player.GetVolume();
        }

        public virtual void SetVolume(int percentage)
        {
            Player.SetVolume(percentage);
        }

        public virtual List<SongInformation> LoadFiles(string[] files)
        {
            return Player.LoadFiles(files);
        }

        public virtual void Dispose()
        {
            Player?.Dispose();
        }

        public virtual int? GetSongPosition()
        {
            return Player.GetSongPosition();
        }

        public virtual void Play(string url)
        {
            Player.Play(url);
        }
    }
}
