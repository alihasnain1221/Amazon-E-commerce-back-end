namespace E_commerce_FYP_backend.Models.Users
{
    public class Users
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
    }
}
