using Microsoft.AspNetCore.Http;

namespace MediaCollection.Services
{
    public interface IPhotoService
    {
        string AddPhoto(IFormFile photo);
        void DeletePhoto(string fileName);
    }
}