using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using MusicPlayer.Extensions;
using MusicPlayer.Models;
using NAudio.CoreAudioApi;
using MusicPlayer.Interface;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// Plays the music
    /// </summary>
    internal class MusicPlayer : IMusicPlayer
    {
        #region Variables

        /// <summary>
        /// Contains the list of songs (absolute paths).
        /// </summary>
        private List<SongInformation> _sourceList;

        /// <summary>
        /// This list stores the played locations in shuffle mode.
        /// </summary>
        private readonly List<string> _locationsPlayed = new List<string>();

        /// <summary>
        ///  audio output.
        /// </summary>
        private WaveOut _waveOutDevice;

        /// <summary>
        /// Indicates whether the class is owned by another class and is only receiving commands.
        /// </summary>
        private readonly bool _isReceiveMode;

        /// <summary>
        /// The boolean indicating whether shuffle is on.
        /// </summary>
        private bool _shuffle;

        /// <summary>
        /// The currently set volume.
        /// </summary>
        private int _volume;

        /// <summary>
        /// Indicates that the class is disposing, all threads should have this var in the while loop.
        /// </summary>
        private bool _disposing;

        /// <summary>
        /// The current song.
        /// </summary>
        private SongInformation _currentSong;

        /// <summary>
        /// The thread for updating the time via the event.
        /// </summary>
        private Thread _timeTracker;

        /// <summary>
        /// The Naudio foundation reader, used to decode audio files.
        /// </summary>
        private MediaFoundationReader _playStream;

        #endregion

        /// <summary>
        /// The songchanged event.
        /// </summary>
        public event SongChanged SongChanged;

        /// <summary>
        /// Initialises the player
        /// </summary>
        public MusicPlayer(bool isReceiveMode = false)
        {
            _volume = DataController.GetSetting(SettingType.Volume, 50);
            _shuffle = DataController.GetSetting(SettingType.Shuffle, false);
            _sourceList = new List<SongInformation>();
            _isReceiveMode = isReceiveMode;
        }

        // <summary>
        /// Gets the current song.
        /// </summary>
        /// <returns>The current song.</returns>
        public SongInformation GetCurrentSong()
        {
            return _currentSong;
        }

        /// Gets the position of the current playing song.
        /// </summary>
        /// <returns>The position in seconds.</returns>
        public int? GetSongPosition()
        {
            if (_waveOutDevice != null && _waveOutDevice.PlaybackState != PlaybackState.Stopped && _playStream != null)
            {
                return Convert.ToInt32(_playStream.CurrentTime.TotalSeconds);
            }

            return null;
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <returns>The current time or null.</returns>
        public TimeSpan? GetCurrentTime()
        {
            return _playStream?.CurrentTime;
        }

        /// <summary>
        /// Pauses or plays the music.
        /// </summary>
        /// <returns>A boolean indicating whether the music is playing.</returns>
        public bool TogglePlay(bool? pause = null)
        {
            var result = false;

            if (_waveOutDevice != null && _currentSong != null)
            {
                if ((_waveOutDevice.PlaybackState == PlaybackState.Playing && pause != false) || pause == true)
                {
                    _currentSong.IsPlaying = false;
                    _waveOutDevice.Pause();
                }
                else if (_playStream != null)
                {
                    _waveOutDevice.Play();
                    _currentSong.IsPlaying = true;
                    _timeTracker?.Abort();

                    if (!_currentSong.IsInternetRadio)
                    {
                        _timeTracker = new Thread(UpdateTime);
                        _timeTracker.Start();
                    }

                    result = true;
                }

                SongChanged?.Invoke(_currentSong);
            }

            return result;
        }

        /// <summary>
        /// Loads a song
        /// </summary>
        /// <param name="song">The song object.</param>
        private void Load(SongInformation song)
        {
            _currentSong = song;

            if (!song.IsInternetLocation)
            {
                Load(song.Location);
            }
            else
            {
                Play(song.Location);
            }
        }

        /// <summary>
        /// Load a song
        /// </summary>
        /// <param name="path">the absolute path</param>
        private void Load(string path)
        {
            try
            {
                DisposeWaveOut(true);
                _playStream?.Dispose();
                _playStream = new MediaFoundationReader(path);

                if (_playStream == null)
                {
                    return;
                }

                _waveOutDevice.Init(_playStream);
                _currentSong.Duration = Convert.ToInt64(_playStream.TotalTime.TotalSeconds);
                SongChanged?.Invoke(_currentSong);
                TogglePlay();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "MusicPlayer: Could not load song " + path);
                _playStream = null;

                if (!_isReceiveMode)
                {
                    Next();
                }
            }
        }

        /// <summary>
        /// Loads all the files in the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>The loaded songs.</returns>
        public List<SongInformation> LoadFolder(string folder)
        {
            var files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
            return LoadFiles(files.ToArray());
        }

        /// <summary>
        /// Gets songs from the currently loaded songs according to the provided paramters.
        /// </summary>
        /// <param name="index">The index to start at.</param>
        /// <param name="query">The query string.</param>
        /// <param name="amount">The maximum amount of songs to return.</param>
        /// <returns>The songs.</returns>
        public List<SongInformation> GetSongs(int index = 0, string query = null, int amount = 50)
        {
            query = query.ToLower();
            var matches = _sourceList?.Where(s => (s.Title != null && s.Title.ToLower().Contains(query)) ||
                                                  (s.Band != null && s.Band.ToLower().Contains(query)) ||
                                                  (s.Location != null && s.Location.ToLower().Contains(query)))
                .OrderBy(s => string.IsNullOrEmpty(query) || !_shuffle ? s.FileName : s.Title).Skip(index).Take(amount).ToList();

            if (matches != null)
            {
                var resolveTasks = new List<Task>();

                for (var i = 0; i < matches.Count; i++)
                {
                    var j = i;

                    var task = Task.Run(() =>
                    {
                        matches[j] = SongInfoController.Resolve(matches[j]);
                        var sourceIndex = _sourceList.FindIndex(s => s.Location == matches[j].Location);
                        _sourceList[sourceIndex] = matches[j];
                    });

                    resolveTasks.Add(task);
                }

                Task.WaitAll(resolveTasks.ToArray());
            }

            return matches;
        }

        /// <summary>
        /// Loads a list of file locations.
        /// </summary>
        /// <param name="files">The file locations.</param>
        /// <param name="number">The number of files to load.</param>
        /// <returns>A List of songs.</returns>
        public List<SongInformation> LoadFiles(string[] files)
        {
            if (_waveOutDevice != null && _waveOutDevice.PlaybackState != PlaybackState.Stopped)
            {
                _waveOutDevice.PlaybackStopped -= OnWaveOutStop;
                _waveOutDevice.Stop();
            }

            var extensions = new[] { ".mp3", ".flac", ".wma", ".aac", ".ogg", ".m4a", ".opus", ".wav" };
            _sourceList = files.Where(path =>
                                        extensions.Contains(Path.GetExtension(path).ToLower())
                                        && path.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                                .Select(path => new SongInformation(path))
                                .OrderBy(s => s.FileName)
                                .ToList();

            return _sourceList;
        }

        /// <summary>
        /// Sets the available songs for the player.
        /// </summary>
        /// <param name="songs">The songs to set.</param>
        public void SetSongs(List<SongInformation> songs)
        {
            _sourceList = songs;
        }

        /// <summary>
        /// Gets the shuffle setting.
        /// </summary>
        /// <returns>The shuffle setting.</returns>
        public bool GetShuffle()
        {
            return _shuffle;
        }

        /// <summary>
        /// Change the shuffle settings.
        /// </summary>
        /// <param name="shuffle">To shuffle or not to shuffle.</param>
        public void SetShuffle(bool shuffle)
        {
            _shuffle = shuffle;
            DataController.SetSetting(SettingType.Shuffle, shuffle);
        }

        /// <summary>
        /// Play a song object.
        /// </summary>
        /// <param name="song">The song.</param>
        public void Play(SongInformation song)
        {
            if (song == null)
            {
                return;
            }

            if (_sourceList.All(ct => ct.Location != song.Location))
            {
                _sourceList.Add(song);
            }

            PlayFromFileLocation(song.Location);
        }

        /// <summary>
        /// Play a song from an online location.
        /// </summary>
        /// <param name="url">The url.</param>
        public void Play(string url)
        {
            DisposeWaveOut(true);
            _playStream?.Dispose();
            try
            {
                var radio = Factory.GetRadioInfo().GetStation(url).Result;
                _currentSong = radio != null ? new SongInformation(radio) : _sourceList.FirstOrDefault(sl => sl.Location == url);

                if (_currentSong == null)
                {
                    throw new ArgumentException("The url must be known");
                }

                Task.Run(() =>
                {
                    _playStream = new MediaFoundationReader(url);
                    _waveOutDevice.Init(_playStream);
                }).Wait(1000);

                TogglePlay(pause: false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Could not connect to url: " + url);
            }
        }

        /// <summary>
        /// Sets the volume of the waveout device.
        /// </summary>
        /// <param name="percentage">The volume in percentage.</param>
        public void SetVolume(int percentage)
        {
            if (percentage > 100 || percentage <= -1)
            {
                return;
            }

            _volume = percentage;

            if (_waveOutDevice == null)
            {
                _waveOutDevice = new WaveOut();
            }

            _waveOutDevice.Volume = percentage / (float)100;
            DataController.SetSetting(SettingType.Volume, percentage);
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public int GetVolume()
        {
            return _volume;
        }

        /// <summary>
        /// Not in use, may be usefull in the future.
        /// </summary>
        /// <returns></returns>
        private static int GetVolumeOfDefaultAudioDevice()
        {
            var devEnum = new MMDeviceEnumerator();
            var defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var leftVolume = (int)(defaultDevice.AudioMeterInformation.PeakValues[0] * 100);
            return leftVolume;
        }

        /// <summary>
        /// Starts a next song from the source list.
        /// </summary>
        public void Next()
        {
            var index = 0;

            if (_sourceList == null || _sourceList.Count <= 0)
            {
                return;
            }

            if (_shuffle)
            {
                var listWithoutPlayed = _sourceList.Where(sl => !_locationsPlayed.Contains(sl.Location)).ToList();

                if (!listWithoutPlayed.Any())
                {
                    _locationsPlayed.Clear();
                    listWithoutPlayed = _sourceList;
                }

                index = new Random().Next(0, listWithoutPlayed.Count);
                index = _sourceList.IndexOf(listWithoutPlayed[index]);
                _locationsPlayed.Add(_sourceList[index].Location);
            }
            else if (_waveOutDevice != null && _currentSong != null)
            {
                _locationsPlayed.Clear();
                index = _sourceList.IndexOf(_currentSong) + 1;
            }

            if (index >= _sourceList.Count)
            {
                index = 0;
            }

            var song = _sourceList[index];
            Play(song);
        }

        /// <summary>
        /// Sets the Song position to a certain time.
        /// </summary>
        /// <param name="seconds">The second to move to.</param>
        public void MoveToTime(long seconds)
        {
            if (_playStream != null && _playStream.TotalTime.TotalSeconds > seconds)
            {
                _playStream.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }

        /// <summary>
        /// Disposes this class properly
        /// </summary>
        public void Dispose()
        {
            try
            {
                _disposing = true;
                DisposeWaveOut();
                _playStream?.Dispose();
            }
            catch
            {
                // TODO: fix other thread exception
            }
        }

        /// <summary>
        /// Updates the position of the song.
        /// </summary>
        private void UpdateTime()
        {
            while (!_disposing && _waveOutDevice != null && _currentSong != null && _playStream != null
                    && (_waveOutDevice.PlaybackState == PlaybackState.Paused || _waveOutDevice.PlaybackState == PlaybackState.Playing))
            {
                try
                {
                    if (_waveOutDevice.PlaybackState == PlaybackState.Playing)
                    {
                        _currentSong.Position = Convert.ToInt64(_playStream.CurrentTime.TotalSeconds);
                        SongChanged?.Invoke(_currentSong);
                    }

                    ThreadExtensions.SaveSleep(1000);
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Next random song from the sourceList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWaveOutStop(object sender, EventArgs e)
        {
            if (!_isReceiveMode && _currentSong.IsPlaying && !_currentSong.IsInternetLocation)
            {
                Next();
            }
        }

        /// <summary>
        /// Disposes of the wave out device.
        /// </summary>
        /// <param name="reinit">A boolean indicating whether to reinitialize the wave out device.</param>
        private void DisposeWaveOut(bool reinit = false)
        {
            if (_waveOutDevice != null && (_waveOutDevice.PlaybackState == PlaybackState.Playing || _waveOutDevice.PlaybackState == PlaybackState.Paused))
            {
                _waveOutDevice.PlaybackStopped -= OnWaveOutStop;

                try
                {
                    _waveOutDevice.Stop();
                }
                catch
                {
                    // ignored
                }

                _waveOutDevice.Dispose();
            }

            if (reinit)
            {
                _waveOutDevice = new WaveOut();
                _waveOutDevice.PlaybackStopped += OnWaveOutStop;
                SetVolume(_volume);
            }
        }

        /// <summary>
        /// The destructor.
        /// </summary>
        /// <remarks>This should not be called, the player should be disposed via the dispose method.</remarks>
        ~MusicPlayer()
        {
            Dispose();
        }

        /// <summary>
        /// Play from key (path).
        /// </summary>
        /// <param name="key"></param>
        private void PlayFromFileLocation(string key)
        {
            var song = _sourceList.FirstOrDefault(ct => ct.Location == key);

            if (song == null)
            {
                return;
            }

            if (!song.IsResolved && !song.IsInternetLocation)
            {
                _ = SongInfoController.Resolve(song);
            }

            Load(song);
        }
    }
}
