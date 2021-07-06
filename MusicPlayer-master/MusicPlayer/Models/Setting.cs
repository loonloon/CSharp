﻿using System.ComponentModel.DataAnnotations;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes a setting.
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        [Key]
        public string Name { get; set; }

        /// <summary>
        /// Gest or sets the value of the setting.
        /// </summary>
        public string Value { get; set; }
    }
}
