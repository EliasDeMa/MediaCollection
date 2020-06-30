using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Models.PodcastModels;
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
    }
}
