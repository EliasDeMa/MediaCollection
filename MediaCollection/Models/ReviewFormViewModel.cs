using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class ReviewFormViewModel
    {
        public int Id { get; set; }
        public string ReviewType { get; set; }

        [Display(Name = "Description")]
        public string NewReview { get; set; }

        [Display(Name = "Score")]
        public int NewReviewScore { get; set; }
    }
}
