using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Data.Migrations;
using MediaCollection.Domain;
using MediaCollection.Models;
using MediaCollection.Models.PlaylistViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PlaylistController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlaylist(AddPlaylistFormViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playlist = new PlayList
            {
                UserId = userId,
                Name = vm.NewPlaylistName
            };

            _applicationDbContext.PlayLists.Add(playlist);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddSong(int songId, int playlistId)
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

            return RedirectToAction("Detail", new { Id = playlistId });
        }

        public async Task<IActionResult> Detail(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var playlist = await _applicationDbContext.PlayLists
                .Include(pl => pl.PlayListSongs)
                .ThenInclude(pls => pls.Song)
                .ThenInclude(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(pl => pl.Id == id);

            var vm = new PlaylistDetailViewModel
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Songs = playlist.PlayListSongs
                .Select(pls => pls.Song)
                .Select(song => new PlaylistSong
                {
                    Id = song.Id,
                    SongTitle = song.Title,
                    BandName = song.Album.Band.Name,
                    AlbumTitle = song.Album.Title,
                    Duration = song.Duration,
                    ReleaseDate = song.Album.ReleaseDate,
                    Url = song.SongLink
                }),
            };

            return View(vm);
        }
    }
}
