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
    public class MusicController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MusicController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var songs = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .ToListAsync();


            return View(songs.Select(song => new MusicIndexViewModel { 
                Id = song.Id,
                SongTitle = song.Title,
                BandName = song.Album.Band.Name,
                AlbumTitle = song.Album.Title
            }));
        }

        public IActionResult Create()
        {
            return View(new MusicCreateViewModel());
        }
    }
}
