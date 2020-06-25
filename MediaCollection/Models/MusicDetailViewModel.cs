using MediaCollection.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class MusicDetailViewModel
    {
        public int Id { get; set; }
        public string SongTitle { get; set; }
        public string AlbumTitle { get; set; }
        public string BandName { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public IEnumerable<SongReviewViewModel> Reviews { get; set; }
        public bool AlreadyReviewed { get; set; }
        public string NewReview { get; set; }
        public int NewReviewScore { get; set; }
    }
}
