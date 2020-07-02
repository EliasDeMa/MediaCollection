using System;
using System.Collections.Generic;

namespace MediaCollection.Domain
{
    public class PodcastEpisode
    {
        public int Id { get; set; }
        public int PodcastId { get; set; }
        public Podcast Podcast { get; set; }
        public string EpisodeTitle { get; set; }
        public int EpisodeNumber { get; set; }
        public string NormalizedEpisodeTitle { get; set; }
        public ICollection<PodcastEpisodeReview> PodcastEpisodeReviews { get; set; }
        public TimeSpan Duration { get; set; }
        public string Link { get; set; }
    }
}