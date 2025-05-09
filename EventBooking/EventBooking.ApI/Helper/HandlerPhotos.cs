namespace EventBooking.ApI.Helper
{
    public static class HandlerPhotos
    {
        private static IWebHostEnvironment _environment;
        private static string _imagePath;


        public static void Initialize(IWebHostEnvironment webHostEnvironment)
        {
            _environment = webHostEnvironment;
            _imagePath = $"{_environment.WebRootPath}";
        }

        public static string UploadPhoto(IFormFile file, string folderName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Invalid image format.";
            }
            string uploadDir = Path.Combine(_imagePath, "images", folderName);
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var path = Path.Combine(_imagePath, "images", folderName, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }

        public static List<string> UploadPhotos(IEnumerable<IFormFile> files, string folderName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var photoPaths = new List<string>();

            if (files == null || !files.Any())
                return photoPaths;

            var uploadDir = Path.Combine(_environment.WebRootPath, "images", folderName);
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    continue; 
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                photoPaths.Add(fileName);
            }

            return photoPaths;
        }
        public static void DeletePhoto(string folderName, string fileName)
        {
            var path = Path.Combine(_imagePath, "Images", folderName, fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        #region DeletePhoto0 Anthor Function 
        public static bool DeletePhoto0(string folderName, string fileName)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, "images", folderName, fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            Console.WriteLine($"File not found at: {fullPath}");
            return false;
        } 
        #endregion
    }
}

