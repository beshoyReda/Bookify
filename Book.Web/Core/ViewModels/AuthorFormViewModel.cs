namespace Book.Web.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int AuthorId { get; set; }
        [MaxLength(30, ErrorMessage = Errors.MaxLenght), Display(Name = "Author"),
            RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.onlyEnglishLetters)]
        [Remote(action: "AllowItem", null!, AdditionalFields = "AuthorId", ErrorMessage = Errors.Duplicated)]
        public string Name { get; set; } = null!;
    }
}
