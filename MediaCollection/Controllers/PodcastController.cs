using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Domain;
using MediaCollection.Models.PodcastModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Controllers
{
    public class PodcastController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PodcastController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var podcasts = (await _applicationDbContext.Podcasts.ToListAsync())
                .Select(podcast => new PodcastIndexViewModel
                {
                    Id = podcast.Id,
                    Name = podcast.Name
                });


            return View(podcasts);
        }

        public IActionResult Create() 
        {
            return View(new PodcastCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(PodcastCreateViewModel vm)
        {
            var podcast = new Podcast
            {
                Name = vm.Name,
                NormalizedName = vm.Name.ToUpper(),
                Description = vm.Description,
            };

            await _applicationDbContext.Podcasts.AddAsync(podcast);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
