using System.Collections.Generic;

namespace MediaCollection.Domain
{
    public class Band
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}