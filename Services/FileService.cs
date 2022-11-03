using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class FileService : IFileService
    {
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        private const int BYTES_PER_UNIT = 1024;

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string base64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64,{base64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal fileSize = bytes;

            while(Math.Round(fileSize / BYTES_PER_UNIT) >= 1)
            {
                fileSize /= bytes;
                counter++;
            }

            return string.Format("{0:n1}{1}", fileSize, suffixes[counter]);
        }

        public string GetFileIcon(string file)
        {
            string fileImage = "default";

            if (string.IsNullOrWhiteSpace(file))
                return fileImage;

            fileImage = Path.GetExtension(file).Replace(".", "");
            return $"/img/contenttype/{fileImage}.png";
        }
    }
}
