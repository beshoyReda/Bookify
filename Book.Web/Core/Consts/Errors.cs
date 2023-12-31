namespace Book.Web.Core.Consts
{
    public static class Errors
    {
        public const string RequiredField = "Required Field";
        public const string MaxLenght = "Length cannot be more than {1} characters";
        public const string Duplicated = "Another Record with the same {0} is already exits!";
        public const string DuplicatedBook = "Book with the same Title is already exits with the same author !";
        public const string NotAllowedExtension = "Only .png , .jpg , .jpeg files are allowed!";
        public const string MaxSize = "file cannot be more than 2 MB!";
        public const string NotAllowFutureDates = "Date cannot be in the future!";
        public const string InvalidRange = "{0} should be between {1} to {2}";
        public const string MaxminLenghth = "The {0} must be at least {2} and at max {1} characters long.";
        public const string ConfirmPasswordNotMatch = "The password and confirmation password do not match.";
        public const string WeekPassWord = "Passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least 8 characters long.";
        public const string InvalidUsername = "Username can only contain letters or digits.";
        public const string onlyEnglishLetters = "Only English letters are allowed.";
        public const string onlyArabicLetters = "Only Arabic letters are allowed.";
        public const string onlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";
        public const string DenySpecialCharacters = "Special characters are not allowed.";
        public const string InValidMobileNumber = "InValid mobile number.";


    }
}
