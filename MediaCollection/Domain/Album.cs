using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Domain
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public int? BandId { get; set; }
#nullable enable
        public Band? Band { get; set; }
#nullable disable
        public DateTime? ReleaseDate { get; set; }
        public ICollection<Song> Songs { get; set; }
        public ICollection<AlbumReview> AlbumReviews { get; set; }
        public string PhotoUrl { get; set; }

    }
}
