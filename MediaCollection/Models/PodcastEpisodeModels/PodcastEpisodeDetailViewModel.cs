using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.PodcastEpisodeModels
{
    public class PodcastEpisodeDetailViewModel
    {
        public string Link { get; internal set; }
        public TimeSpan Duration { get; internal set; }
        public string Title { get; internal set; }
    }
}
