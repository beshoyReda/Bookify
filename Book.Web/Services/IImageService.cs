namespace Book.Web.Services
{
	public interface IImageService
	{
		Task<(bool isUploaded, string? errorMessage)> uploadAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail);

		void Delete(string imagePath,string? imageThumbnailPath = null);
	}
}
