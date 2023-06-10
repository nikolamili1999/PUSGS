namespace IdentityApi.Dto
{
    public class GoogleLoginDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long Birthday { get; set; }
        public string Address { get; set; }
        public string PhotoUrl { get; set; }
    }
}
