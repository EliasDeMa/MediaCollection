using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Domain
{
    public class PodcastReview
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public string Description { get; set; }

        [Range(0, 10)]
        public int Score { get; set; }

        public int PodcastId { get; set; }
        public Podcast Podcast { get; set; }

        public string UserId { get; set; }
        public MediaCollectionUser User { get; set; }
    }
}
