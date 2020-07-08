using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.AlbumViewModels
{
    public class AlbumReviewViewModel
    {
        public string User { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; }
        public int Score { get; set; }
        public int Id { get; set; }
    }
}
