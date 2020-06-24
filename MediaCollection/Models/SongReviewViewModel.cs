using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class SongReviewViewModel
    {
        public string User { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
    }
}
