using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.PodcastEpisodeModels
{
    public class PodcastEpisodeIndexViewModel
    {
        public int Id { get; set; }
        public string EpisodeName { get; set; }
        public string Podcast { get; set; }
        public string Link { get; set; }
    }
}
