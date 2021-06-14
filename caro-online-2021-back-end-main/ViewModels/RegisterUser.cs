namespace CaroOnline2021.ViewModels
{
    public class RegisterUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool IsAdmin { get; set; }

        public string Name { get; set; }
    }
}
