using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Domain;
using MediaCollection.Models;
using MediaCollection.Models.SongViewModels;
using MediaCollection.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace MediaCollection.Controllers
{
    public class SongController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ISongService _songService;

        public SongController(ApplicationDbContext applicationDbContext, ISongService songService)
        {
            _applicationDbContext = applicationDbContext;
            _songService = songService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var songs = await _songService.GetSongs();

            var bands = await _applicationDbContext.Bands.ToListAsync();
            var albums = await _applicationDbContext.Albums.ToListAsync();

            List<SelectListItem> bandSelectList, albumSelectList;
            ToSelectList(bands, albums, out bandSelectList, out albumSelectList);

            var userPlaylists = await GetUserPlayLists(userId);

            var songModels = new List<SongIndexViewModel>();

            foreach (var song in songs)
            {
                if (song.Hidden)
                {
                    songModels.Add(new SongIndexViewModel
                    {
                        Hidden = true,
                        Id = song.Id,
                    });
                }
                else
                {
                    var model = new SongIndexViewModel
                    {
                        Id = song.Id,
                        Hidden = false,
                        SongTitle = song.Title,
                        Duration = song.Duration,
                    };

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
            }

            var vm = new MusicIndexViewModel
            {
                Songs = songModels,
                PlayLists = userPlaylists,
                BandNames = bandSelectList,
                AlbumTitles = albumSelectList,
            };

            return View(vm);
        }

        private static void ToSelectList(List<Band> bands, List<Album> albums, out List<SelectListItem> bandSelectList, out List<SelectListItem> albumSelectList)
        {
            bandSelectList = bands.Select(band => new SelectListItem
            {
                Value = band.Id.ToString(),
                Text = band.Name
            }).ToList();
            bandSelectList.Insert(0, new SelectListItem { Value = "0", Text = "None" });

            albumSelectList = albums.Select(album => new SelectListItem
            {
                Value = album.Id.ToString(),
                Text = album.Title
            }).ToList();
            albumSelectList.Insert(0, new SelectListItem { Value = "0", Text = "None" });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SongFilterViewModel vm)
        {
            var selectedBand = (await _applicationDbContext.Bands
                .FirstOrDefaultAsync(band => band.Id == vm.SelectedBand))?.Name ?? "";

            var selectedAlbum = (await _applicationDbContext.Albums
                .FirstOrDefaultAsync(album => album.Id == vm.SelectedAlbum))?.Title ?? "";

            if (vm.SelectedAlbum == 0 && vm.SelectedBand == 0)
            {
                return RedirectToAction("Index");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bands = await _applicationDbContext.Bands.ToListAsync();
            var albums = await _applicationDbContext.Albums.ToListAsync();
            List<SelectListItem> bandSelectList, albumSelectList;
            ToSelectList(bands, albums, out bandSelectList, out albumSelectList);

            var songs = await _songService.GetSongs();

            IEnumerable<Song> albumsFiltered = vm.SelectedAlbum != 0 
                ? songs.Where(song => song.Album.Title == selectedAlbum)
                : songs;

            IEnumerable<Song> allFiltered = vm.SelectedBand != 0
                ? albumsFiltered.Where(song => song.Album.Band.Name == selectedBand)
                : albumsFiltered;

            var userPlaylists = await GetUserPlayLists(userId);

            var songModels = new List<SongIndexViewModel>();

            foreach (var song in allFiltered)
            {
                if (song.Hidden)
                {
                    songModels.Add(new SongIndexViewModel
                    {
                        Hidden = true,
                        Id = song.Id,
                    });
                }
                else
                {
                    var model = new SongIndexViewModel
                    {
                        Id = song.Id,
                        Hidden = false,
                        SongTitle = song.Title,
                        Duration = song.Duration,
                    };

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
            }

            var newVm = new MusicIndexViewModel
            {
                Songs = songModels,
                PlayLists = userPlaylists,
                BandNames = bandSelectList,
                AlbumTitles = albumSelectList,
            };

            return View(newVm);
        }

        public IActionResult Create()
        {
            return View(new SongCreateViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(SongCreateViewModel vm)
        {
            DotNetEnv.Env.Load();

            var songToDb = new Song
            {
                Title = vm.SongTitle,
                NormalizedTitle = vm.SongTitle.ToUpper(),
                Duration = vm.Duration,
                SongLink = vm.Link.Replace(DotNetEnv.Env.GetString("YOUTUBE_LINK"), "")
            };

            await _songService.ChangeAlbum(vm.AlbumTitle, songToDb);

            if (songToDb.Album != null && songToDb.Album.ReleaseDate == null && vm.ReleaseDate.HasValue)
            {
                songToDb.Album.ReleaseDate = vm.ReleaseDate.Value;
            }

            await _songService.ChangeBand(vm.BandName, songToDb);
            await _applicationDbContext.Songs.AddAsync(songToDb);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id, bool alreadyReviewed = false)
        {
            var song = await _songService.FindSongByIdDetailed(id);

            var approvedReviews = new List<SongReviewViewModel>();

            var vm = new MusicDetailViewModel
            {
                Id = song.Id,
                AlreadyReviewed = alreadyReviewed,
                SongTitle = song.Title,
                AlbumTitle = song.Album?.Title,
                BandName = song.Album?.Band?.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album?.ReleaseDate,
                Link = song.SongLink
            };

            foreach (var item in song.SongReviews)
            {
                if (item.Approved)
                {
                    approvedReviews.Add(new SongReviewViewModel
                    {
                        Description = item.Description,
                        Score = item.Score,
                        User = item.User.UserName,
                        Id = item.Id,
                        Approved = true,
                    });
                }
                else
                {
                    approvedReviews.Add(new SongReviewViewModel
                    {
                        Description = item.Description,
                        Score = item.Score,
                        User = item.User.UserName,
                        Id = item.Id,
                        Approved = false,
                    });
                }
            }

            vm.Reviews = approvedReviews;

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

            DotNetEnv.Env.Load();

            return View(new SongEditViewModel
            {
                SongTitle = song.Title,
                AlbumTitle = song.Album?.Title,
                BandName = song.Album?.Band?.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album?.ReleaseDate,
                Link = !string.IsNullOrEmpty(song.SongLink) 
                    ? new StringBuilder(DotNetEnv.Env.GetString("YOUTUBE_LINK"))
                        .Append(song.SongLink).ToString() 
                    : ""
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, SongEditViewModel vm)
        {
            var origSong = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            origSong.Duration = vm.Duration;
            DotNetEnv.Env.Load();
            origSong.SongLink = vm.Link.Replace(DotNetEnv.Env.GetString("YOUTUBE_LINK"), "");

            if (origSong.NormalizedTitle != vm.SongTitle.ToUpper())
            {
                origSong.Title = vm.SongTitle;
                origSong.NormalizedTitle = vm.SongTitle.ToUpper();
            }

            if (origSong.Album?.NormalizedTitle != vm.AlbumTitle)
            {
                await _songService.ChangeAlbum(vm.AlbumTitle, origSong);
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (origSong.Album != null && !origSong.Album.ReleaseDate.HasValue)
            {
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (origSong.Album?.Band?.NormalizedName != vm.BandName)
            {
                await _songService.ChangeBand(vm.BandName, origSong);
            }

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { Id = id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            return View(new SongDeleteViewModel
            {
                Id = id,
                SongTitle = song.Title,
                BandName = song.Album?.Band?.Name
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

        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> ToggleHide(int id)
        {
            var song = await _applicationDbContext.Songs.FindAsync(id);

            song.Hidden = !song.Hidden;

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
