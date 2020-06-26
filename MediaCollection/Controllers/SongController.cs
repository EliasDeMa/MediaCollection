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

        public IActionResult Create()
        {
            return View(new SongCreateViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(SongCreateViewModel vm)
        {
            IQueryable<Band> bandExists = null;
            IQueryable<Album> albumExists = null;

            if (!string.IsNullOrEmpty(vm.AlbumTitle))
            {
                albumExists = _applicationDbContext.Albums.Where(album => album.NormalizedTitle == vm.AlbumTitle.ToUpper());
            }

            if (!string.IsNullOrEmpty(vm.BandName))
            {
                bandExists = _applicationDbContext.Bands.Where(band => band.NormalizedName == vm.BandName.ToUpper());
            }

            var songToDb = new Song
            {
                Title = vm.SongTitle,
                NormalizedTitle = vm.SongTitle.ToUpper(),
                Duration = vm.Duration
            };

            await ChangeAlbum(vm.AlbumTitle, albumExists, songToDb);

            if (songToDb.Album != null && songToDb.Album.ReleaseDate == null && vm.ReleaseDate.HasValue)
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

        private static async Task ChangeAlbum(string albumTitle, IQueryable<Album> albumExists, Song songToDb)
        {
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
                AlbumTitle = song.Album?.Title,
                BandName = song.Album?.Band?.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album?.ReleaseDate,
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

        public async Task<IActionResult> Edit(int id)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            return View(new SongEditViewModel
            {
                SongTitle = song.Title,
                AlbumTitle = song.Album?.Title,
                BandName = song.Album?.Band?.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album?.ReleaseDate
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, SongEditViewModel vm)
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

            if (origSong.Album?.NormalizedTitle != vm.AlbumTitle)
            {
                await ChangeAlbum(vm.AlbumTitle, albumExists, origSong);
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (!origSong.Album.ReleaseDate.HasValue)
            {
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (origSong.Album?.Band?.NormalizedName != vm.BandName)
            {
                await ChangeBand(vm.BandName, bandExists, origSong);
            }

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { Id = id });
        }
    }
}
