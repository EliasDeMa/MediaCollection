using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Domain
{
    public class SongReview
    {
        public int Id { get; set; }
        public string Description { get; set; }

        [Range(0, 10)]
        public int Score { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }
    }
}
