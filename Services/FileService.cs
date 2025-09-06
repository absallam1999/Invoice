using invoice.Core.Interfaces.Services;

namespace invoice.Services
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly string[] _allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".bmp",
            ".webp",
        };
        private readonly long _maxFileSize = 5 * 1024 * 1024;

        public FileService(
            IConfiguration configuration,
            IHostEnvironment environment
        )
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task<bool> DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return true;

            if (Uri.TryCreate(imagePath, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return true;
            }

            var relative = imagePath.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);

            var wwwroot = Path.Combine(_environment.ContentRootPath, "wwwroot");
            var physicalPath = Path.Combine(wwwroot, relative);

            try
            {
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }

                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        public string GetImageUrl(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return imagePath!;

            if (Uri.TryCreate(imagePath, UriKind.Absolute, out var _))
                return imagePath;

            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? _configuration["BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "http://localhost";
            }

            baseUrl = baseUrl.TrimEnd('/');
            var relative = imagePath.TrimStart('~', '/');

            return $"{baseUrl}/{relative}";
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null) return false;

            if (file.Length <= 0 || file.Length > _maxFileSize)
                return false;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_allowedExtensions.Contains(ext))
                return false;

            if (!string.IsNullOrEmpty(file.ContentType) && !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        public async Task<string> UpdateImageAsync(IFormFile newFile, string oldImagePath, string controllerName)
        {
            if (newFile == null)
                return oldImagePath;

            if (!IsValidImageFile(newFile))
                throw new ArgumentException("Invalid image file. Check extension and size.");

            var newPath = await UploadImageAsync(newFile, controllerName);

            if (!string.IsNullOrWhiteSpace(oldImagePath))
            {
                _ = await DeleteImageAsync(oldImagePath);
            }

            return newPath;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string controllerName)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (!IsValidImageFile(file))
                throw new ArgumentException("Invalid image file. Allowed extensions: " + string.Join(", ", _allowedExtensions));

            controllerName = string.IsNullOrWhiteSpace(controllerName) ? "common" : controllerName.Trim().ToLowerInvariant();

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{ext}";

            var uploadsDir = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", controllerName);

            try
            {
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var physicalPath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relative = Path.Combine("uploads", controllerName, fileName).Replace(Path.DirectorySeparatorChar, '/');
                return relative;
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to upload image.", ex);
            }
        }
    }
}
