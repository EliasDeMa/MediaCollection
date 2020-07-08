using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Domain;
using MediaCollection.Models.PlaylistViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CommentController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> AddComment(int id, AddPlaylistCommentViewModel vm)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _applicationDbContext.PlaylistComments.AddAsync(new PlaylistComment
            {
                UserId = userId,
                Content = vm.Content,
                PlaylistId = id
            });

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Playlist", new { Id = id });
        }
    }
}
