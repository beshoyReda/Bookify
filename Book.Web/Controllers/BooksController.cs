using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;

namespace Book.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        private List<string> _allowedExtensions = new List<string>() { ".jpg", ".png", "jpeg" };
        private int _maxAllowedSize = 2097152;
        public BooksController(ApplicationDbContext context, IMapper mapper,
            IWebHostEnvironment webHostEnviroment,
            IOptions<CloudinarySettings> cloudinary)
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
                var extension = Path.GetExtension(model.Image.FileName);
                if(!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image),Errors.NotAllowedExtension);
                    model = PopulateViewModel(model);
                    return View("Form", model);
                }
                if(model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    model = PopulateViewModel(model);
                    return View("Form", model);
                }
                var imageName = $"{Guid.NewGuid()}{extension}";

                var path = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", imageName);
                var thumbpath = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books/thumb", imageName);

                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
                stream.Dispose();
                
                book.ImageUrl = $"/images/books/{imageName}";
                book.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";

                //goz2 el khas bl package sixLabors
                using var image = Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = (float)image.Height / ratio;
                image.Mutate(i => i.Resize(width: 200,height:(int)height));
                image.Save(thumbpath);

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
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.BookId == model.BookId);
            
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
                    var oldImagePath = $"{_webHostEnviroment.WebRootPath}{book.ImageUrl}";
                    var oldImageThumbPath = $"{_webHostEnviroment.WebRootPath}{book.ImageThumbnailUrl}";

                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);

                    if (System.IO.File.Exists(oldImageThumbPath))
                        System.IO.File.Delete(oldImageThumbPath);

                    //await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);
                }

                var extension = Path.GetExtension(model.Image.FileName);
                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                    model = PopulateViewModel(model);
                    return View("Form", model);
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    model = PopulateViewModel(model);
                    return View("Form", model);
                }
                var imageName = $"{Guid.NewGuid()}{extension}";

                var path = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", imageName);
                var thumbpath = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books/thumb", imageName);

                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
                stream.Dispose();

                model.ImageUrl = $"/images/books/{imageName}";
                model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";

                //goz2 el khas bl package sixLabors
                using var image = Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = (float)image.Height / ratio;
                image.Mutate(i => i.Resize(width: 200, height: (int)height));
                image.Save(thumbpath);




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
            book.LastUpdatedOn = DateTime.Now;
            //add thumbnailUrl
            //book.ImageThumbnailUrl=GetThumbnailUrl(book.ImageUrl!);
            //book.ImagePublicId = imagePublicId;


            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
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
