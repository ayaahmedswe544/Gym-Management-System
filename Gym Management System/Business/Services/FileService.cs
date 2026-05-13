using Gym_Management_System.Business.IService;

namespace Gym_Management_System.Business.Services
{
    public class FileService:IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", subFolder);
            Directory.CreateDirectory(uploadsFolder);
            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/{subFolder}/{uniqueName}";
        }

        public void DeleteFile(string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl)) return;

            var rootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var filePath = Path.Combine(rootPath, relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
