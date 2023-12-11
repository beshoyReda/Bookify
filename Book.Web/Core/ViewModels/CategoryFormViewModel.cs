namespace Book.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int CategoryId { get; set; }
        [MaxLength(30,ErrorMessage="Max Length Cannot be more than 30")]
        public string Name { get; set; } = null!;
    }
}
