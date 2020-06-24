using System;
using System.Collections.Generic;

namespace MediaCollection.Domain
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public TimeSpan Duration { get; set; }

        public int? AlbumId { get; set; }
        public Album Album { get; set; }
        public ICollection<PlayListSong> PlayListSongs { get; set; }
        public ICollection<SongReview> SongReviews { get; set; }
    }
}