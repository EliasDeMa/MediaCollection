using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediaCollection.Models;
using MediaCollection.Data;
using Microsoft.EntityFrameworkCore;
using MediaCollection.Models.HomeViewModels;
using MediaCollection.Models.SongViewModels;
using MediaCollection.Domain;
using Z.EntityFramework.Plus;
using MediaCollection.Models.PodcastModels;
using MediaCollection.Models.PodcastEpisodeModels;

namespace MediaCollection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var topTenSongs = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(song => song.Band)
                .IncludeFilter(song => song.SongReviews.Where(review => review.PostDate > DateTime.Now.AddDays(-10)))
                .OrderByDescending(song => song.SongReviews.Count)
                .Take(10)
                .ToListAsync();

            var topTenPodcastEpisodes = await _applicationDbContext.PodcastEpisodes
                .Include(pe => pe.Podcast)
                .IncludeFilter(pe => pe.PodcastEpisodeReviews.Where(review => review.PostDate > DateTime.Now.AddDays(-10)))
                .OrderByDescending(song => song.PodcastEpisodeReviews.Count)
                .Take(10)
                .ToListAsync();

            var songModels = new List<SongIndexViewModel>();

            foreach (var song in topTenSongs)
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
                        if (song.Album.Band != null)
                        {
                            model.BandName = song.Album.Band.Name;
                        }
                    }

                    songModels.Add(model);
                }
            }

            var vm = new HomeIndexViewModel
            {
                TopTenSongs = songModels,
                TopTenPodcasts = topTenPodcastEpisodes.Select(pe => new PodcastEpisodeIndexViewModel
                {
                    Id = pe.Id,
                    EpisodeName = pe.EpisodeTitle,
                    Podcast = pe.Podcast.Name
                })
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
