using MediaCollection.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Models.SongViewModels
{
    public class SongFilterViewModel
    {
        [Display(Name = "Select Band")]
        public int SelectedBand { get; set; }
        [Display(Name = "Select Album")]
        public int SelectedAlbum { get; set; }
        public IEnumerable<SelectListItem> BandNames { get; set; }
        public IEnumerable<SelectListItem> AlbumTitles { get; set; }
    }
}
