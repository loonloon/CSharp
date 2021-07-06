﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes a song.
    /// </summary> 
    /// <remarks>
    /// Properties with the datamember attribute are send over the WCF service.
    /// Properties with the jsonproperty atttribute are available in the UI.
    /// </remarks>
    [Serializable]
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class SongInformation
    {
        [Key]
        [StringLength(255)]
        [JsonProperty]
        public string Location { get; set; }

        [JsonProperty]
        public DateTime? DateAdded { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Title { get; set; }

        [StringLength(512)]
        [DataMember]
        public string FileName { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Band { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Album { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Genre { get; set; }

        [DataMember]
        [JsonProperty]
        public DateTime? DateCreated { get; set; }

        [JsonProperty]
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the duration in seconds.
        /// </summary>
        [DataMember]
        [JsonProperty]
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the current position of the song.
        /// </summary>
        [JsonProperty]
        public long Position { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [DataMember]
        [JsonProperty]
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the image url.
        /// </summary>
        [DataMember]
        [JsonProperty]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the song is playing.
        /// </summary>
        [JsonProperty]
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Gets or sets the file stream.
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is an internet radio.
        /// </summary>
        [JsonProperty]
        public bool IsInternetRadio { get; set; }

        /// <summary>
        /// Gets or sets a boolean indicating whether the song is resolved.
        /// </summary>
        [NotMapped]
        internal bool IsResolved { get; set; }

        /// <summary>
        /// Gets a value indicating whether the song is stored on the internet.
        /// </summary>
        [NotMapped]
        internal bool IsInternetLocation => Uri.IsWellFormedUriString(Location, UriKind.Absolute);

        /// <summary>
        /// Creates a new empty song.
        /// </summary>
        public SongInformation() { }

        /// <summary>
        /// Creates a song from a file path.
        /// </summary>
        /// <param name="path">The file path.</param>
        public SongInformation(string path)
        {
            Location = path; ;
            var temp = path.Split('\\').Last();
            var t2 = temp.Split('-');

            if (t2.Length > 1)
            {
                Title = t2.Last();
                Band = t2.First();
            }
            else
            {
                Title = t2.First();
            }
        }

        /// <summary>
        /// Initializes the song information from a radio station.
        /// </summary>
        /// <param name="radio">The internet radio station.</param>
        public SongInformation(RadioStation radio)
        {
            Location = radio.Url;
            Title = radio.Name;
            Genre = radio.Genre;
            ImageUrl = radio.ImageUrl;
            IsInternetRadio = true;
            IsResolved = true;
            Position = 0;
        }
    }
}
