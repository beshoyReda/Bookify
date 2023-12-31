using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Signing;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;

namespace Book.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]

    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IImageService _imageService;

        private List<string> _allowedExtensions = new List<string>() { ".jpg", ".png", "jpeg" };
        private int _maxAllowedSize = 2097152;
		public BooksController(ApplicationDbContext context, IMapper mapper,
			IWebHostEnvironment webHostEnviroment,
			IOptions<CloudinarySettings> cloudinary, IImageService imageService)
		{
			_context = context;
			_mapper = mapper;
			_webHostEnviroment = webHostEnviroment;

			var account = new Account()
			{
				Cloud = cloudinary.Value.Cloud,
				ApiKey = cloudinary.Value.ApiKey,
				ApiSecret = cloudinary.Value.ApiSecret
			};
			_cloudinary = new Cloudinary(account);
			_imageService = imageService;
		}

		public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetBooks()
        {
            var skip = int.Parse(Request.Form["start"]);
            var pageSize = int.Parse(Request.Form["length"]);

            var searchValue = Request.Form["search[value]"];

            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];


            IQueryable<Core.Models.Book> books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .ThenInclude(c => c.Category);

            if(!string.IsNullOrEmpty(searchValue))
            {
                books = books.Where(b => b.Title.Contains(searchValue) || b.Author!.Name.Contains(searchValue));
            }

            books = books.OrderBy($"{sortColumn} {sortColumnDirection}");

            var data = books.Skip(skip).Take(pageSize).ToList();
            
            var mappedData = _mapper.Map<IEnumerable<BookViewModel>>(data);

            var recordsTotal = books.Count();

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }

        public IActionResult Details(int id)
        {

            var book = _context.Books
                .Include(b => b.Author)
                .Include(c => c.Copies)
                .Include(b => b.Categories)
                .ThenInclude(c => c.Category)
                .SingleOrDefault(b => b.BookId == id);

            if (book is null)
                return NotFound();

            var viewModel = _mapper.Map<BookViewModel>(book);

            return View(viewModel);
        }
        public IActionResult Create()
        {

            var viewModel = PopulateViewModel();


            return View("Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(BookFormViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                model= PopulateViewModel(model);
                return View("Form", model);
            }
            var book = _mapper.Map<Core.Models.Book>(model);

            if (model.Image is not null)
            {
				var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.uploadAsync(model.Image, imageName, folderPath:"/images/books",hasThumbnail: true);

                if (!isUploaded)
                {
					ModelState.AddModelError(nameof(Image), errorMessage!);
					return View("Form", PopulateViewModel(model));
				}

				book.ImageUrl = $"/images/books/{imageName}";
				book.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";


				//using var stream = model.Image.OpenReadStream();

				//var imageParams = new ImageUploadParams
				//{
				//    File = new FileDescription(imageName, stream),
				//    //To can handle the file name in cloudinary 
				//    UseFilename = true,

				//    //Transformation = new Transformation()
				//    //.Height(300)
				//    //.Width(500)
				//    //.Radius("max")
				//    //.Gravity("face")
				//    //.Crop("fill")
				//};

				//var result = await _cloudinary.UploadAsync(imageParams);
				//book.ImageUrl = result.SecureUrl.ToString();
				//book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl);
				//book.ImagePublicId = result.PublicId;
			}

			book.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            
            _context.Books.Add(book); 
            _context.SaveChanges();

            return RedirectToAction(nameof(Details),new { id = book.BookId});


        }
        public  IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b=>b.Categories).SingleOrDefault(b=>b.BookId==id);
            
            if (book is null)
                return NotFound();

            
            var model = _mapper.Map<BookFormViewModel>(book);
            var viewModel = PopulateViewModel(model);

            viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();

            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(BookFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model = PopulateViewModel(model);
                return View("Form", model);
            }
            var book = _context.Books
                .Include(b => b.Categories)
                .Include(b => b.Copies)
                .SingleOrDefault(b => b.BookId == model.BookId);
            
            if (book is null)
                return NotFound();

            //string imagePublicId = null;

            if (model.Image is not null)
            {
                //bshof lw feh sora asln gowa el database wla la b3d kda bshof lw mafesh b7ot el sora w akml el code
                //w le feh b3ml delete ll place holder w a7ot el sora el gdeda 
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    //var oldImagePath = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", book.ImageUrl);
                    _imageService.Delete(book.ImageUrl,book.ImageThumbnailUrl);

                    //await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);
                }
				var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";

				var (isUploaded, errorMessage) = await _imageService.uploadAsync(model.Image, imageName, folderPath: "/images/books", hasThumbnail: true);

				if (!isUploaded)
				{
					ModelState.AddModelError(nameof(Image), errorMessage!);
					return View("Form", PopulateViewModel(model));
				}

				model.ImageUrl = $"/images/books/{imageName}";
				model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";

				//var path = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", imageName);

				//using var stream = System.IO.File.Create(path);
				//await model.Image.CopyToAsync(stream);

				//model.ImageUrl = imageName;


				//using var stream = model.Image.OpenReadStream() ;

				//var imageParams = new ImageUploadParams
				//{
				//    File = new FileDescription(imageName, stream),
				//    UseFilename = true
				//};
				//var result = await _cloudinary.UploadAsync(imageParams);
				//model.ImageUrl = result.SecureUrl.ToString();
				//imagePublicId = result.PublicId;

			}
            else if (model.Image is null && !string.IsNullOrEmpty(book.ImageUrl))
            {
                model.ImageUrl = book.ImageUrl;
                model.ImageThumbnailUrl = book.ImageThumbnailUrl;
            }


            book = _mapper.Map(model, book);
            book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            book.LastUpdatedOn = DateTime.Now;
            //add thumbnailUrl
            //book.ImageThumbnailUrl=GetThumbnailUrl(book.ImageUrl!);
            //book.ImagePublicId = imagePublicId;


            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            if(!model.IsAvailableForRental)
            {
                foreach(var copy in book.Copies)
                {
                    copy.IsAvailableForRental = false;
                }
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = book.BookId });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var book = _context.Books.Find(id);

            if (book is null)
                return NotFound();
            //if (category.IsDeleted)
            //    category.IsDeleted = false;
            //else
            //    category.IsDeleted = true;
            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            book.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();

            return Ok();

        }
        public IActionResult AllowItem(BookFormViewModel model)
        {
            var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId==model.AuthorId);
            var isAllowed = book is null || book.BookId.Equals(model.BookId);

            return Json(isAllowed);
        }
        private BookFormViewModel PopulateViewModel(BookFormViewModel? model=null) 
        {
            BookFormViewModel viewModel = model is null ? new BookFormViewModel():model;

            var authors = _context.Authors.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
            var categories = _context.Categories.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();

            viewModel.Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);
            
            return viewModel;
        }
        //private string GetThumbnailUrl(string url)
        //{
        //    var separator = "image/upload/";
        //    var urlParts = url.Split(separator);
            
        //    var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";
            
        //    return thumbnailUrl;
        //}
    }
}
