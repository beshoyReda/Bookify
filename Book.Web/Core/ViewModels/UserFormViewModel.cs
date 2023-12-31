using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Book.Web.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLenght),Display(Name ="Full Name"),
            RegularExpression(RegexPatterns.CharactersOnly_Eng,ErrorMessage =Errors.onlyEnglishLetters)]
        
        public string FullName { get; set; }=null!;
        
        
        [MaxLength(50, ErrorMessage = Errors.MaxLenght),Display(Name = "Username"),
            RegularExpression(RegexPatterns.Username, ErrorMessage = Errors.InvalidUsername)]
        [Remote(action: "AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]

        
        public string UserName { get; set; } = null!;
        
        
        [MaxLength(200, ErrorMessage = Errors.MaxLenght), EmailAddress]
        [Remote(action: "AllowEmail", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]

        public string Email { get; set; } =null!;


        [StringLength(100, ErrorMessage =Errors.MaxminLenghth, MinimumLength = 8),
            DataType(DataType.Password),
            RegularExpression(RegexPatterns.Password,ErrorMessage =Errors.WeekPassWord),
            Display(Name = "Password")]
        [RequiredIf("Id == null",ErrorMessage = Errors.RequiredField)]
        public string? Password { get; set; } = null!;


        [DataType(DataType.Password),
        Display(Name = "Confirm password"),
        Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredField)]
        public string? ConfirmPassword { get; set; } = null!;
        

        [Display(Name = "Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
