using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace caro.ViewModels
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
