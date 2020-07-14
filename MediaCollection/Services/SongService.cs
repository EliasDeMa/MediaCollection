using MediaCollection.Data;
using MediaCollection.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Services
{
    public class SongService : ISongService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SongService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Song>> GetSongs()
            => await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .ToListAsync();

        public async Task<Song> FindSongByIdDetailed(int id)
            => await _applicationDbContext.Songs
                .Include(song => song.SongReviews)
                .ThenInclude(review => review.User)
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

        public async Task ChangeBand(string bandName, Song songToDb)
        {
            var bandExists = !string.IsNullOrEmpty(bandName) 
                ? _applicationDbContext.Bands.Where(album => album.NormalizedName == bandName.ToUpper())
                : null;

            if (bandExists == null)
            {
                return;
            }
            else if (bandExists.Any() && songToDb.Album.BandId == null)
            {
                songToDb.Album.Band = await bandExists.FirstOrDefaultAsync();
            }
            else if (!bandExists.Any())
            {
                songToDb.Album.Band = new Band
                {
                    Name = bandName,
                    NormalizedName = bandName.ToUpper()
                };
            }
        }

        public async Task ChangeAlbum(string albumTitle, Song songToDb)
        {
            var albumExists = !string.IsNullOrEmpty(albumTitle)
                ? _applicationDbContext.Albums.Where(album => album.NormalizedTitle == albumTitle.ToUpper())
                : null;

            if (albumExists == null)
            {
                return;
            }
            else if (albumExists.Any())
            {
                songToDb.Album = await albumExists.FirstOrDefaultAsync();
            }
            else
            {
                songToDb.Album = new Album
                {
                    Title = albumTitle,
                    NormalizedTitle = albumTitle.ToUpper()
                };
            }
        }
    }
}
