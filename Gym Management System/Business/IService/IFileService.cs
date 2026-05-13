namespace Gym_Management_System.Business.IService
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string subFolder);
        void DeleteFile(string relativeUrl);
    }
}
