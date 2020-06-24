using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Domain;
using MediaCollection.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Controllers
{
    public class MusicController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MusicController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PlayListIndex()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userPlaylists = await _applicationDbContext.PlayLists
                .Where(pl => pl.UserId == userId)
                .ToListAsync();

            return View(new PlaylistIndexViewModel
            {
                PlayLists = userPlaylists.Select(item => new PlayListIndividualViewModel
                {
                    Id = item.Id,
                    Name = item.Name
                })
            });

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlayListIndex(PlaylistIndexViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playlist = new PlayList
            {
                UserId = userId,
                Name = vm.NewPlayListName
            };

            _applicationDbContext.PlayLists.Add(playlist);
            await _applicationDbContext.SaveChangesAsync();

            var userPlaylists = await _applicationDbContext.PlayLists
                .Where(pl => pl.UserId == userId)
                .ToListAsync();

            return View(new PlaylistIndexViewModel
            {
                PlayLists = userPlaylists.Select(item => new PlayListIndividualViewModel
                {
                    Id = item.Id,
                    Name = item.Name
                })
            });
        }

        public async Task<IActionResult> SongIndex()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var songs = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .ToListAsync();

            var userPlaylists = (await _applicationDbContext.PlayLists
                .Where(pl => pl.UserId == userId)
                .ToListAsync())
                .Select(item => new PlayListIndividualViewModel { 
                    Id = item.Id,
                    Name = item.Name
                });

            return View(songs.Select(song => new MusicIndexViewModel
            {
                Id = song.Id,
                SongTitle = song.Title,
                BandName = song.Album.Band.Name,
                AlbumTitle = song.Album.Title,
                Duration = song.Duration,
                ReleaseDate = song.Album.ReleaseDate,
                PlayLists = userPlaylists
            }));
        }

        public async Task<IActionResult> AddSongToPlayList(int songId, int playlistId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playlist = await _applicationDbContext.PlayLists
                .FirstOrDefaultAsync(pl => pl.Id == playlistId && pl.UserId == userId);

            if (playlist.PlayListSongs == null)
            {
                playlist.PlayListSongs = new List<PlayListSong>();
            }

            playlist.PlayListSongs.Add(new PlayListSong
            {
                SongId = songId
            });

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("SongIndex");
        }

        #region Create

        public IActionResult Create()
        {
            return View(new MusicCreateViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(MusicCreateViewModel vm)
        {
            var bandExists = _applicationDbContext.Bands.Where(band => band.NormalizedName == vm.BandName.ToUpper());
            var albumExists = _applicationDbContext.Albums.Where(album => album.NormalizedTitle == vm.AlbumTitle.ToUpper());

            var songToDb = new Song
            {
                Title = vm.SongTitle,
                NormalizedTitle = vm.SongTitle.ToUpper(),
                Duration = vm.Duration
            };

            await ChangeAlbum(vm.AlbumTitle, albumExists, songToDb);

            if (songToDb.Album.ReleaseDate == null && vm.ReleaseDate.HasValue)
            {
                songToDb.Album.ReleaseDate = vm.ReleaseDate.Value;
            }

            await ChangeBand(vm.BandName, bandExists, songToDb);

            await _applicationDbContext.Songs.AddAsync(songToDb);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private static async Task ChangeBand(string bandName, IQueryable<Band> bandExists, Song songToDb)
        {
            if (bandExists.Any() && songToDb.Album.BandId == null)
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

        private static async Task ChangeAlbum(string albumTitle, IQueryable<Album> albumExists, Song songToDb)
        {
            if (albumExists.Any())
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

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int id)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            var vm = new MusicDetailViewModel
            {
                SongTitle = song.Title,
                AlbumTitle = song.Album.Title,
                BandName = song.Album.Band.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album.ReleaseDate
            };

            return View(vm);
        }

        #endregion

        #region Edit

        public async Task<IActionResult> Edit(int id)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            return View(new MusicEditViewModel
            {
                SongTitle = song.Title,
                AlbumTitle = song.Album.Title,
                BandName = song.Album.Band.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album.ReleaseDate
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MusicEditViewModel vm)
        {
            var bandExists = _applicationDbContext.Bands.Where(band => band.NormalizedName == vm.BandName.ToUpper());
            var albumExists = _applicationDbContext.Albums.Where(album => album.NormalizedTitle == vm.AlbumTitle.ToUpper());
            var origSong = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            origSong.Duration = vm.Duration;

            if (origSong.NormalizedTitle != vm.SongTitle.ToUpper())
            {
                origSong.Title = vm.SongTitle;
                origSong.NormalizedTitle = vm.SongTitle.ToUpper();
            }

            if (origSong.Album.NormalizedTitle != vm.AlbumTitle)
            {
                await ChangeAlbum(vm.AlbumTitle, albumExists, origSong);
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (!origSong.Album.ReleaseDate.HasValue)
            {
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (origSong.Album.Band.NormalizedName != vm.BandName)
            {
                await ChangeBand(vm.BandName, bandExists, origSong);
            }

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { Id = id });
        }

        #endregion

        #region Delete

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            return View(new MusicDeleteViewModel
            {
                Id = id,
                SongTitle = song.Title,
                BandName = song.Album.Band.Name
            });
        }

        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var song = await _applicationDbContext.Songs.FindAsync(id);

            _applicationDbContext.Songs.Remove(song);

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion
    }
}
