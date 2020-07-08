using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Domain
{
    public class PlaylistComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PlaylistId { get; set; }
        public PlayList PlayList { get; set; }

        public string UserId { get; set; }
        public MediaCollectionUser User { get; set; }
    }
}
