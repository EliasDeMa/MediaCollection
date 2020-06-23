﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaCollection.Data;
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
                AlbumTitle = song.Album.Title,
                Duration = song.Duration,
                ReleaseDate = song.Album.ReleaseDate
            }));
        }

        #region Create

        public IActionResult Create()
        {
            return View(new MusicCreateViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(MusicCreateViewModel vm)
        {
            var bandExists = _applicationDbContext.Bands.Where(band => band.NormalizedName == vm.BandName.ToUpper());
            var albumExists = _applicationDbContext.Albums.Where(album => album.NormalizedTitle == vm.AlbumTitle.ToUpper());

            var songToDb = new Song
            {
                Title = vm.SongTitle,
                NormalizedTitle = vm.SongTitle.ToUpper(),
                Duration = vm.Duration
            };

            await ChangeAlbum(vm.AlbumTitle, albumExists, songToDb);

            if (songToDb.Album.ReleaseDate == null && vm.ReleaseDate.HasValue)
            {
                songToDb.Album.ReleaseDate = vm.ReleaseDate.Value;
            }

            await ChangeBand(vm.BandName, bandExists, songToDb);

            await _applicationDbContext.Songs.AddAsync(songToDb);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private static async Task ChangeBand(string bandName, IQueryable<Band> bandExists, Song songToDb)
        {
            if (bandExists.Any() && songToDb.Album.BandId == null)
            {
                songToDb.Album.Band = await bandExists.FirstOrDefaultAsync();
            }
            else if (!bandExists.Any())
            {
                songToDb.Album.Band = new Band
                {
                    Name = bandName,
                    NormalizedName = bandName.ToUpper()
                };
            }
        }

        private static async Task ChangeAlbum(string albumTitle, IQueryable<Album> albumExists, Song songToDb)
        {
            if (albumExists.Any())
            {
                songToDb.Album = await albumExists.FirstOrDefaultAsync();
            }
            else
            {
                songToDb.Album = new Album
                {
                    Title = albumTitle,
                    NormalizedTitle = albumTitle.ToUpper()
                };
            }
        }

        #endregion

        #region Detail

        public async Task<IActionResult> Detail(int id)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            var vm = new MusicDetailViewModel
            {
                SongTitle = song.Title,
                AlbumTitle = song.Album.Title,
                BandName = song.Album.Band.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album.ReleaseDate
            };

            return View(vm);
        }

        #endregion

        #region Edit

        public async Task<IActionResult> Edit(int id)
        {
            var song = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            return View(new MusicEditViewModel { 
                SongTitle = song.Title,
                AlbumTitle = song.Album.Title,
                BandName = song.Album.Band.Name,
                Duration = song.Duration,
                ReleaseDate = song.Album.ReleaseDate
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MusicEditViewModel vm)
        {
            var bandExists = _applicationDbContext.Bands.Where(band => band.NormalizedName == vm.BandName.ToUpper());
            var albumExists = _applicationDbContext.Albums.Where(album => album.NormalizedTitle == vm.AlbumTitle.ToUpper());
            var origSong = await _applicationDbContext.Songs
                .Include(song => song.Album)
                .ThenInclude(album => album.Band)
                .FirstOrDefaultAsync(item => item.Id == id);

            origSong.Duration = vm.Duration;

            if (origSong.NormalizedTitle != vm.SongTitle.ToUpper())
            {
                origSong.Title = vm.SongTitle;
                origSong.NormalizedTitle = vm.SongTitle.ToUpper();
            }

            if (origSong.Album.NormalizedTitle != vm.AlbumTitle)
            {
                await ChangeAlbum(vm.AlbumTitle, albumExists, origSong);
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (!origSong.Album.ReleaseDate.HasValue)
            {
                origSong.Album.ReleaseDate = vm.ReleaseDate;
            }

            if (origSong.Album.Band.NormalizedName != vm.BandName)
            {
                await ChangeBand(vm.BandName, bandExists, origSong);
            }

            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { Id = id });
        }

        #endregion
    }
}
