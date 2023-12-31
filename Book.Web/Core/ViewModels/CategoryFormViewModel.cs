namespace Book.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int CategoryId { get; set; }
        [MaxLength(30, ErrorMessage = Errors.MaxLenght), Display(Name = "Category"),
            RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.onlyEnglishLetters)]
        [Remote("AllowItem", null!, AdditionalFields = "CategoryId", ErrorMessage = Errors.Duplicated)]
        public string Name { get; set; } = null!;
    }
}
