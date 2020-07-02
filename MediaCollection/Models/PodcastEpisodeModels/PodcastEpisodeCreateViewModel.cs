using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.PodcastEpisodeModels
{
    public class PodcastEpisodeCreateViewModel
    {
        public int EpisodeNumber { get; set; }
        public string EpisodeName { get; set; }
        public TimeSpan Duration { get; set; }
        public int PodcastId { get; set; }
        public string Url { get; set; }
    }
}
