using Web.Controllers;

namespace Web.Models
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public UserData User { get; set; }
    }
}
