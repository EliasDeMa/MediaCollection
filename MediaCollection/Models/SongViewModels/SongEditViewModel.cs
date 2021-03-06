﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class SongEditViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Songtitle cannot be empty")]
        [Display(Name = "Song title")]
        public string SongTitle { get; set; }

        [Display(Name = "Album title")]
        public string AlbumTitle { get; set; }

        [Display(Name = "Band name")]
        public string BandName { get; set; }

        [Display(Name = "Release date")]
        public DateTime? ReleaseDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Duration cannot be zero")]
        public TimeSpan Duration { get; set; }
        public string Link { get; set; }
        public IFormFile PhotoUrl { get; set; }

    }
}
