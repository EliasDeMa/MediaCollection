using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediaCollection.Data;
using MediaCollection.Data.Migrations;
using MediaCollection.Domain;
using MediaCollection.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Controllers
{
    public class MusicController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MusicController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }


        #region Detail

        public async Task<IActionResult> AlbumDetail(int id)
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

        #endregion
    }
}
