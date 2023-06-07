using IdentityApi.Models;

namespace IdentityApi.Dto
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long Birthday { get; set; }
        public string Address { get; set; }
        public EUserRole Role { get; set; }
        public ERequestStatus SellerStatus { set; get; }
    }
}
