using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Domain;
using MediaCollection.Models;
using MediaCollection.Models.PodcastEpisodeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReviewController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(ReviewFormViewModel vm)
        {
            return vm.ReviewType switch
            {
                nameof(Song) => RedirectToAction("AddSongReview", vm),
                nameof(Album) => RedirectToAction("AddAlbumReview", vm),
                nameof(PodcastEpisode) => RedirectToAction("AddPodcastEpisodeReview", vm),
                _ => RedirectToAction("Index", "Music"),
            };
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult EditReview(ReviewFormViewModel vm)
        {
            return vm.ReviewType switch
            {
                nameof(Song) => RedirectToAction("EditSongReview", vm),
                nameof(Album) => RedirectToAction("EditAlbumReview", vm),
                nameof(PodcastEpisode) => RedirectToAction("EditPodcastEpisodeReview", vm),
                _ => RedirectToAction("Index", "Music"),
            };
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
                    UserId = userId,
                    Approved = false
                };

                _applicationDbContext.SongReviews.Add(songReview);
                await _applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "Song", new { vm.Id });
            }
            else
            {
                return RedirectToAction("Detail", "Song", new { vm.Id, AlreadyReviewed = true });
            }
        }

        [Authorize]
        public async Task<IActionResult> AddAlbumReview(ReviewFormViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userReviews = _applicationDbContext.AlbumReviews
                .Where(x => x.AlbumId == vm.Id && userId == x.UserId);

            if (!userReviews.Any())
            {
                var albumReview = new AlbumReview
                {
                    Description = vm.NewReview,
                    Score = vm.NewReviewScore,
                    AlbumId = vm.Id,
                    UserId = userId
                };

                _applicationDbContext.AlbumReviews.Add(albumReview);
                await _applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "Album", new { vm.Id });
            }
            else
            {
                return RedirectToAction("Detail", "Album", new { vm.Id, AlreadyReviewed = true });
            }
        }


        public async Task<IActionResult> AddPodcastEpisodeReview(ReviewFormViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userReviews = _applicationDbContext.PodcastEpisodeReviews
                .Where(x => x.PodcastEpisodeId == vm.Id && userId == x.UserId);

            if (!userReviews.Any())
            {
                var podcastEpisodeReview = new PodcastEpisodeReview
                {
                    Description = vm.NewReview,
                    Score = vm.NewReviewScore,
                    PodcastEpisodeId = vm.Id,
                    UserId = userId
                };

                _applicationDbContext.PodcastEpisodeReviews.Add(podcastEpisodeReview);
                await _applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "PodcastEpisode", new { vm.Id });
            }
            else
            {
                return RedirectToAction("Detail", "PodcastEpisode", new { vm.Id, AlreadyReviewed = true });
            }
        }


        public async Task<IActionResult> EditSongReview(ReviewFormViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var origReview = await _applicationDbContext.SongReviews
                .FirstOrDefaultAsync(review => review.UserId == userId && review.SongId == vm.Id);

            origReview.Score = vm.NewReviewScore;
            origReview.Description = vm.NewReview;
            origReview.Approved = false;

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Song", new { vm.Id });
        }

        [Authorize]
        public async Task<IActionResult> EditAlbumReview(ReviewFormViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var origReview = await _applicationDbContext.AlbumReviews
                .FirstOrDefaultAsync(review => review.UserId == userId && review.AlbumId == vm.Id);

            origReview.Score = vm.NewReviewScore;
            origReview.Description = vm.NewReview;
            origReview.Approved = false;

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Album", new { vm.Id });
        }

        [Authorize]
        public async Task<IActionResult> EditPodcastEpisodeReview(ReviewFormViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var origReview = await _applicationDbContext.PodcastEpisodeReviews
                .FirstOrDefaultAsync(review => review.UserId == userId && review.PodcastEpisodeId == vm.Id);

            origReview.Score = vm.NewReviewScore;
            origReview.Description = vm.NewReview;
            origReview.Approved = false;

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "PodcastEpisode", new { vm.Id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveSongReview(int songId, int reviewId)
        {
            var review = await _applicationDbContext.SongReviews.FindAsync(reviewId);

            review.Approved = true;

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Song", new { Id = songId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveAlbumReview(int albumId, int reviewId)
        {
            var review = await _applicationDbContext.AlbumReviews.FindAsync(reviewId);

            review.Approved = true;

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Album", new { Id = albumId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovePodcastEpisodeReview(int podcastEpisodeId, int reviewId)
        {
            var review = await _applicationDbContext.PodcastEpisodeReviews.FindAsync(reviewId);

            review.Approved = true;

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "PodcastEpisode", new { Id = podcastEpisodeId });
        }
    }
}
