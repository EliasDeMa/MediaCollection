using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Domain
{
    public class AlbumReview
    {
        public int Id { get; set; }
        public string Description { get; set; }

        [Range(0, 10)]
        public int Score { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public string UserId { get; set; }
        public MediaCollectionUser User { get; set; }
    }
}
