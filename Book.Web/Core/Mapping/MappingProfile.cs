using AutoMapper;

namespace Book.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Category 
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();

        }
    }
}
