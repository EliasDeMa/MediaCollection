﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class MusicDetailViewModel
    {
        public string SongTitle { get; set; }
        public string AlbumTitle { get; set; }
        public string BandName { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}