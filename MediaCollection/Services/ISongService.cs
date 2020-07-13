using MediaCollection.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaCollection.Services
{
    public interface ISongService
    {
        Task<List<Song>> GetSongs();
        Task<Song> FindSongByIdDetailed(int id);
    }
}