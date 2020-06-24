using MediaCollection.Domain;
using System.Collections.Generic;

namespace MediaCollection.Models
{
    public class PlaylistDetailViewModel
    {
        public string Name { get; set; }
        public IEnumerable<MusicIndexViewModel> Songs { get; set; }
    }
}