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


        #region Detail

        public async Task<IActionResult> PlaylistDetail(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playlist = await _applicationDbContext.PlayLists
                .Include(pl => pl.PlayListSongs)
                .ThenInclude(pls => pls.Song)
                .ThenInclude(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(pl => pl.Id == id);

            var userPlaylists = await GetUserPlayLists(userId);

            var vm = new PlaylistDetailViewModel
            {
                Name = playlist.Name,
                Songs = playlist.PlayListSongs
                .Select(pls => pls.Song)
                .Select(song => new MusicIndexViewModel
                {
                    Id = song.Id,
                    SongTitle = song.Title,
                    BandName = song.Album.Band.Name,
                    AlbumTitle = song.Album.Title,
                    Duration = song.Duration,
                    ReleaseDate = song.Album.ReleaseDate,
                    PlayLists = userPlaylists
                }),
            };

            return View(vm);
        }

        public async Task<IActionResult> AlbumDetail(int id)
        {
            var album = await _applicationDbContext.Albums
                .Include(album => album.Band)
                .Include(album => album.Songs)
                .FirstOrDefaultAsync(album => album.Id == id);

            return View(new AlbumDetailViewModel
            {
                Id = album.Id,
                Band = album.Band.Name,
                Title = album.Title,
                Songs = album.Songs.Select(song => (song.Title, song.Duration))
            }); 
        }

        private async Task<IEnumerable<PlayListIndividualViewModel>> GetUserPlayLists(string userId)
        {
            return (await _applicationDbContext.PlayLists
                .Where(pl => pl.UserId == userId)
                .ToListAsync())
                .Select(item => new PlayListIndividualViewModel
                {
                    Id = item.Id,
                    Name = item.Name
                });
        }

        #endregion
    }
}
