using MediaCollection.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.SongViewModels
{
    public class SongFilterViewModel
    {
        public int SelectedBand { get; set; }
        public int SelectedAlbum { get; set; }
        public IEnumerable<SelectListItem> BandNames { get; set; }
        public IEnumerable<SelectListItem> AlbumTitles { get; set; }
    }
}
