using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.SongViewModels
{
    public class SongIndexViewModel
    {
        public int Id { get; set; }
        public int? AlbumId { get; set; }
        public string SongTitle { get; set; }
        public string BandName { get; set; }
        public string AlbumTitle { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool Hidden { get; set; }
    }
}
