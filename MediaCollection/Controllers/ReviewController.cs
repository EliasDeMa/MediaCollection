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

namespace MediaCollection.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReviewController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult AddReview(ReviewFormViewModel vm)
        {
            switch (vm.ReviewType)
            {
                case "Song":
                    return RedirectToAction("AddSongReview", vm); ;
                case "Album":
                    return RedirectToAction("AddAlbumReview", vm);
                default:
                    return RedirectToAction("Index", "Music");
            }
        }


        [Authorize]
        public async Task<IActionResult> AddSongReview(ReviewFormViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userReviews = _applicationDbContext.SongReviews
                .Where(x => x.SongId == vm.Id && userId == x.UserId);

            if (!userReviews.Any())
            {
                var songReview = new SongReview
                {
                    Description = vm.NewReview,
                    Score = vm.NewReviewScore,
                    SongId = vm.Id,
                    UserId = userId
                };

                _applicationDbContext.SongReviews.Add(songReview);
                await _applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "Song", new { Id = vm.Id });
            }
            else
            {
                return RedirectToAction("Detail", "Song", new { Id = vm.Id, AlreadyReviewed = true });
            }
        }

        public async Task<IActionResult> AddAlbumReview(ReviewFormViewModel vm)
        {
            return View();
        }
    }
}
