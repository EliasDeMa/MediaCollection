using MediaCollection.Models.SongViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models
{
    public class MusicIndexViewModel
    {
        public IEnumerable<SongIndexViewModel> Songs { get; set; }
        public IEnumerable<PlayListIndividualViewModel> PlayLists { get; set; }

        public int SelectedBand { get; set; }
        public int SelectedAlbum { get; set; }
        public IEnumerable<SelectListItem> BandNames { get; set; }
        public IEnumerable<SelectListItem> AlbumTitles { get; set; }
    }
}
