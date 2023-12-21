namespace Book.Web.Core.Consts
{
    public static class Errors
    {
        public const string MaxLenght = "Length cannot be more than {1} characters";
        public const string Duplicated = "{0} with the same name is already exits!";
        public const string DuplicatedBook = "Book with the same Title is already exits with the same author !";
        public const string NotAllowedExtension = "Only .png , .jpg , .jpeg files are allowed!";
        public const string MaxSize = "file cannot be more than 2 MB!";
        public const string NotAllowFutureDates = "Date cannot be in the future!";
        public const string InvalidRange = "{0} should be between {1} to {2}";
    }
}
