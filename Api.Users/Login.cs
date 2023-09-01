namespace Api.Users
{
    public class LoginRequest
    {
        public string? Loginname { get; set; }

        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public int Id { get; set; }

        public string? Token { get; set; }
    }
}