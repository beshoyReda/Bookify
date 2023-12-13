namespace Book.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Author : BaseModel
    {
        public int AuthorId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
