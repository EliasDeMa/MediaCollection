using MediaCollection.Models.AlbumViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class AlbumDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Band { get; set; }
        public IEnumerable<(string, TimeSpan)> Songs { get; set; }
        public ReviewFormViewModel ReviewForm { get; set; }
        public bool AlreadyReviewed { get; set; }
        public IEnumerable<AlbumReviewViewModel> Reviews { get; set; }
    }
}
