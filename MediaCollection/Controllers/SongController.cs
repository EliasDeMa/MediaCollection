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
    public class SongController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SongController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var songs = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .ToListAsync();

            var userPlaylists = await GetUserPlayLists(userId);

            List<MusicIndexViewModel> songModels = new List<MusicIndexViewModel>();

            foreach (var song in songs)
            {
                var model = new MusicIndexViewModel();
                model.Id = song.Id;
                model.SongTitle = song.Title;
                model.Duration = song.Duration;
                model.PlayLists = userPlaylists;
                if (song.Album != null)
                {
                    model.AlbumId = song.AlbumId;
                    model.AlbumTitle = song.Album.Title;
                    model.ReleaseDate = song.Album.ReleaseDate;
                    if (song.Album.Band != null)
                    {
                        model.BandName = song.Album.Band.Name;
                    }
                }

                songModels.Add(model);
            }

            return View(songModels);
        }

        public async Task<IActionResult> Detail(int id, bool alreadyReviewed = false)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.SongReviews)
                .ThenInclude(review => review.User)
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            var vm = new MusicDetailViewModel
            {
                Id = song.Id,
                AlreadyReviewed = alreadyReviewed,
                SongTitle = song.Title,
                AlbumTitle = song.Album.Title,
                BandName = song.Album.Band.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album.ReleaseDate,
                Reviews = song.SongReviews.Select(review => new SongReviewViewModel
                {
                    Description = review.Description,
                    Score = review.Score,
                    User = review.User.UserName
                }),
                ReviewForm = new ReviewFormViewModel { Id = song.Id }
            };

            return View(vm);
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
    }
}
