using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class ReviewFormViewModel
    {
        public int Id { get; set; }
        public string NewReview { get; set; }
        public int NewReviewScore { get; set; }
    }
}
