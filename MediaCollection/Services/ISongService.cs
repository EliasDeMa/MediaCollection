using MediaCollection.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Services
{
    public interface ISongService
    {
        Task<List<Song>> GetSongs();
        Task<Song> FindSongByIdDetailed(int id);

        Task ChangeBand(string bandName, Song songToDb);
        Task ChangeAlbum(string albumTitle, Song songToDb);
    }
}