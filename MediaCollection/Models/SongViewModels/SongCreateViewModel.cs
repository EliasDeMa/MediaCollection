using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class SongCreateViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Songtitle cannot be empty")]
        public string SongTitle { get; set; }
        public string AlbumTitle { get; set; }
        public string BandName { get; set; }
        public DateTime? ReleaseDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Duration cannot be zero")]
        public TimeSpan Duration { get; set; }
        public string Link { get; set; }
    }
}
