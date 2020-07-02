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
                .Include(ep => ep.PodcastEpisodeReviews)
                .ThenInclude(review => review.User)
                .FirstOrDefaultAsync(ep => ep.Id == id);

            var approvedReviews = new List<PodcastEpisodeReviewViewModel>();

            foreach (var item in episode.PodcastEpisodeReviews)
            {
                if (item.Approved)
                {
                    approvedReviews.Add(new PodcastEpisodeReviewViewModel
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
                    approvedReviews.Add(new PodcastEpisodeReviewViewModel
                    {
                        Description = item.Description,
                        Score = item.Score,
                        User = item.User.UserName,
                        Id = item.Id,
                        Approved = false,
                    });
                }
            }

            return View(new PodcastEpisodeDetailViewModel
            {
                Id = id,
                Link = episode.Link,
                Duration = episode.Duration,
                Title = episode.EpisodeTitle,
                AlreadyReviewed = alreadyReviewed,
                Reviews = approvedReviews
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
