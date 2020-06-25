using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Models;
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
                .Include(album => album.Band)
                .Include(album => album.Songs)
                .FirstOrDefaultAsync(album => album.Id == id);

            return View(new AlbumDetailViewModel
            {
                Id = album.Id,
                Band = album.Band.Name,
                Title = album.Title,
                Songs = album.Songs.Select(song => (song.Title, song.Duration))
            });
        }
    }
}
