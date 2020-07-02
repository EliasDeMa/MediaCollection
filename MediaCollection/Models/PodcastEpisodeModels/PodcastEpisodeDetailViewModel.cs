using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.PodcastEpisodeModels
{
    public class PodcastEpisodeDetailViewModel
    {
        public int Id { get; set; }
        public string Link { get; internal set; }
        public TimeSpan Duration { get; internal set; }
        public string Title { get; internal set; }
        public bool AlreadyReviewed { get; internal set; }
    }
}
