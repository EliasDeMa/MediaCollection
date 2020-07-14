using MediaCollection.Domain;
using MediaCollection.Models.PodcastModels;
using MediaCollection.Models.SongViewModels;
using System.Collections.Generic;

namespace MediaCollection.Models.HomeViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<SongIndexViewModel> TopTenSongs { get; set; }
        public IEnumerable<PodcastIndexViewModel> TopTenPodcasts { get; set; }
    }
}