namespace Book.Web.Core.ViewModels
{
    public class ResetPasswordFormViewModel
    {
        public string Id { get; set; } = null!;

        [DataType(DataType.Password),
            StringLength(100,ErrorMessage =Errors.MaxminLenghth,MinimumLength =8),
            RegularExpression(RegexPatterns.Password,ErrorMessage =Errors.WeekPassWord)]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password), Display(Name = "Confirm Password"),
            Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch)]
        public string ConfirmPassword { get; set; } = null!;

    }
}
