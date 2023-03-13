namespace JolDos2.Services
{
    public interface IFileService
    {
        Tuple<int, string> SaveImage(IFormFile file);
        public bool DeleteImage(string imageFileName);
    }
}
