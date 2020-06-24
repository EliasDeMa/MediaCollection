namespace MediaCollection.Domain
{
    public class PlayListSong
    {
        public int SongId { get; set; }
        public Song Song { get; set; }
        public int PlayListId { get; set; }
        public PlayList PlayList { get; set; }
    }
}