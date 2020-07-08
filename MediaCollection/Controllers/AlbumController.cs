using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Models;
using MediaCollection.Models.AlbumViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AlbumController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var album = await _applicationDbContext.Albums
                .Include(album => album.AlbumReviews)
                .ThenInclude(albumReview => albumReview.User)
                .Include(album => album.Band)
                .Include(album => album.Songs)
                .FirstOrDefaultAsync(album => album.Id == id);

            var vm = new AlbumDetailViewModel
            {
                Id = album.Id,
                Band = album.Band.Name,
                Title = album.Title,
                Songs = album.Songs.Select(song => (song.Title, song.Duration))
            };

            var approvedReviews = new List<AlbumReviewViewModel>();

            foreach (var item in album.AlbumReviews)
            {
                if (item.Approved)
                {
                    approvedReviews.Add(new AlbumReviewViewModel
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
                    approvedReviews.Add(new AlbumReviewViewModel
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
    }
}
