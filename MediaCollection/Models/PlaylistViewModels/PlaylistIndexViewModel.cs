using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class PlaylistIndexViewModel
    {
        public string NewPlayListName { get; set; }
        public IEnumerable<PlayListIndividualViewModel> PlayLists { get; set; }
    }
}
