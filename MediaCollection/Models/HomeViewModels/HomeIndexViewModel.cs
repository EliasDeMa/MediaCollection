using MediaCollection.Domain;
using MediaCollection.Models.SongViewModels;
using System.Collections.Generic;

namespace MediaCollection.Models.HomeViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<SongIndexViewModel> topTenSongs { get; set; }
    }
}