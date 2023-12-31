using Book.Web.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
namespace Book.Web.Services
{
	public class ImageService : IImageService
	{
		private readonly IWebHostEnvironment _webHostEnviroment;
		private List<string> _allowedExtensions = new List<string>() { ".jpg", ".png", "jpeg" };
		private int _maxAllowedSize = 2097152;
		public ImageService(IWebHostEnvironment webHostEnviroment)
		{
			_webHostEnviroment = webHostEnviroment;
		}

		public async Task<(bool isUploaded, string? errorMessage)> uploadAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail)
		{
			var extension = Path.GetExtension(image.FileName);
			if (!_allowedExtensions.Contains(extension))
				return (isUploaded:false, errorMessage :Errors.NotAllowedExtension);
			
			if (image.Length > _maxAllowedSize)
				return (isUploaded: false, errorMessage: Errors.MaxSize);


			var path = Path.Combine($"{_webHostEnviroment.WebRootPath}{folderPath}", imageName);

			using var stream =File.Create(path);
			await image.CopyToAsync(stream);
			stream.Dispose();

			if(hasThumbnail)
			{
				var thumbpath = Path.Combine($"{_webHostEnviroment.WebRootPath}{folderPath}/thumb", imageName);

				//goz2 el khas bl package sixLabors
				using var loadedImage = Image.Load(image.OpenReadStream());
				var ratio = (float)loadedImage.Width / 200;
				var height = (float)loadedImage.Height / ratio;
				loadedImage.Mutate(i => i.Resize(width: 200, height: (int)height));
				loadedImage.Save(thumbpath);
			}

			return (isUploaded: true, errorMessage: null);

		}
		public void Delete(string imagePath, string? imageThumbnailPath = null)
		{
			var oldImagePath = $"{_webHostEnviroment.WebRootPath}{imagePath}";

			if (File.Exists(oldImagePath))
				File.Delete(oldImagePath);

			if (!string.IsNullOrEmpty(imageThumbnailPath))
			{
				var oldImageThumbPath = $"{_webHostEnviroment.WebRootPath}{imageThumbnailPath}";
				
				if (File.Exists(oldImageThumbPath))
					File.Delete(oldImageThumbPath);
			}


		}
	}
}
