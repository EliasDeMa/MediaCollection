using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaCollection.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public PhotoService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public string AddPhoto(IFormFile photo)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var path = Path.Combine(_hostEnvironment.WebRootPath, "imgs", uniqueFileName);

            using var stream = new FileStream(path, FileMode.Create);
            photo.CopyTo(stream);

            return "/imgs/" + uniqueFileName;
        }

        public void DeletePhoto(string fileName)
        {
            var prevPath = Path.Combine(_hostEnvironment.WebRootPath, "imgs", fileName.Replace("/imgs/", ""));
            System.IO.File.Delete(prevPath);
        }
    }
}
