﻿namespace Book.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category : BaseModel
    {
        public int CategoryId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;

    }
}
