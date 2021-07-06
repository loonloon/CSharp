using System;
using YoutubeExplode.Models;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Class describing video info.
    /// </summary>
    [Serializable]
    public class VideoInfo
    {
        /// <summary>
        /// Gets or sets the id of the video.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the title of the video.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sest the number of times the video was viewed.
        /// </summary>
        public long? ViewCount { get; set; }

        /// <summary>
        /// Gets or sets the duration of the video.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the url of the thumbnail.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the url of the video.
        /// </summary>
        public string Url { get; set; }

        public VideoInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the video from a YoutubeExplode model.
        /// </summary>
        /// <param name="video">The YoutubeExplode model.</param>
        public VideoInfo(Video video)
        {
            ID = video.Id;
            Title = video.Title;
            ViewCount = video.Statistics.ViewCount;
            ThumbnailUrl = video.Thumbnails.HighResUrl;
            Duration = video.Duration;
            Url = $"https://www.youtube.com/watch?v={video.Id}";
        }
    }
}
