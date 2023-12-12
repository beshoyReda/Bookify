using Microsoft.AspNetCore.Mvc;

namespace Book.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int CategoryId { get; set; }
        [MaxLength(30,ErrorMessage="Max Length Cannot be more than 30")]
        [Remote("AllowItem",null,AdditionalFields = "CategoryId", ErrorMessage ="Category with the same name is already Exists!")]
        public string Name { get; set; } = null!;
    }
}
