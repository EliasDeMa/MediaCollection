using MediaCollection.Data;
using MediaCollection.Domain;
using MediaCollection.Models.PodcastEpisodeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Controllers
{
    public class PodcastEpisodeController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PodcastEpisodeController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Detail(int id, bool alreadyReviewed = false)
        {
            var episode = await _applicationDbContext.PodcastEpisodes
                .Include(ep => ep.Podcast)
                .FirstOrDefaultAsync(ep => ep.Id == id);

            return View(new PodcastEpisodeDetailViewModel
            {
                Link = episode.Link,
                Duration = episode.Duration,
                Title = episode.EpisodeTitle,
                AlreadyReviewed = alreadyReviewed
            });
        }

        public IActionResult Create(int id)
        {
            return View(new PodcastEpisodeCreateViewModel
            {
                PodcastId = id
            });
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(int id, PodcastEpisodeCreateViewModel vm)
        {
            var episode = new PodcastEpisode
            {
                EpisodeNumber = vm.EpisodeNumber,
                EpisodeTitle = vm.EpisodeName,
                NormalizedEpisodeTitle = vm.EpisodeName.ToUpper(),
                Duration = vm.Duration,
                Link = vm.Url,
                PodcastId = id
            };

            await _applicationDbContext.PodcastEpisodes.AddAsync(episode);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Podcast", new { Id = id });
        }
    }
}
