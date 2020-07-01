using MediaCollection.Models.PodcastEpisodeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.PodcastModels
{
    public class PodcastDetailViewModel
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public IEnumerable<PodcastEpisodeIndexViewModel> Episodes { get; internal set; }
    }
}
