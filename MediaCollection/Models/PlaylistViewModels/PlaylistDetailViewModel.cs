using MediaCollection.Domain;
using MediaCollection.Models.PlaylistViewModels;
using System.Collections.Generic;

namespace MediaCollection.Models
{
    public class PlaylistDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PlaylistSong> Songs { get; set; }
        public IEnumerable<PlaylistCommentViewModel> PlaylistComments { get; set; }
    }
}