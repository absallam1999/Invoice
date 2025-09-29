namespace invoice.Core.Interfaces.Services
{
    public interface IFileService
    {
        string GetImageUrl(string imagePath);
        bool IsValidImageFile(IFormFile file);

        Task<string> UploadImageAsync(IFormFile file, string controllerName);
        Task<string> UpdateImageAsync(IFormFile newFile, string oldImagePath, string controllerName);
        Task<bool> DeleteImageAsync(string imagePath);
    }
}
