using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Book.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            //Categories 
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();
            CreateMap<Category, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));



            //Author
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();
            CreateMap<Author, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            //Books
            CreateMap<BookFormViewModel, Core.Models.Book>()
                .ReverseMap()
                // el hta bt3t forMember lazm tkon b3d el reverse map w deh 3shan hta el e5tyar bt3t el category !!
                .ForMember(dest => dest.Categories, options => options.Ignore());

            CreateMap<Core.Models.Book, BookViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
                .ForMember(dest => dest.Categories,opt => opt.MapFrom(src => src.Categories.Select(c => c.Category!.Name).ToList()))
                .ReverseMap();

            //BooksCopy
            CreateMap<BookCopy, BookCopyViewModel>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book!.Title))
                .ReverseMap();

            CreateMap<BookCopy, BookCopyFormViewModel>();

            //Users
            CreateMap<ApplicationUser, UserViewModel>();

            CreateMap<UserFormViewModel, ApplicationUser>()
                .ForMember(dest=>dest.NormalizedEmail,opt=>opt.MapFrom(src=>src.Email.ToUpper()))
                .ForMember(dest=>dest.NormalizedUserName,opt=>opt.MapFrom(src=>src.UserName.ToUpper()))
                .ReverseMap();
        }
    }
}
