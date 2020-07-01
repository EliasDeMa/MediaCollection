using MediaCollection.Data;
using MediaCollection.Models.PodcastEpisodeModels;
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

        public async Task<IActionResult> Detail(int id)
        {
            var episode = await _applicationDbContext.PodcastEpisodes
                .Include(ep => ep.Podcast)
                .FirstOrDefaultAsync(ep => ep.Id == id);

            return View(new PodcastEpisodeDetailViewModel
            {
                Link = episode.Link,
                Duration = episode.Duration,
                Title = episode.EpisodeTitle
            });
        }
    }
}
