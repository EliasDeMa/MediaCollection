using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Domain
{
    public class PlayList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public MediaCollectionUser User { get; set; }
        public ICollection<PlayListSong> PlayListSongs { get; set; }
        public ICollection<PlaylistComment> PlaylistComments { get; set; }
    }
}
