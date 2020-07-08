using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.PlaylistViewModels
{
    public class AddPlaylistCommentViewModel
    {
        public int PlaylistId { get; set; }
        public string Content { get; set; }
    }
}
