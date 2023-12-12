namespace Book.Web.Core.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } 
        public DateTime? LastUpdatedOn { get; set; }
    }
}
