namespace NewsBackend.Data
{
    public class UserStorage
    {
        public string? Name { get; set; } 
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }

        public string? Roles { get; set; }
        public string? Email { get; set; }
    }
}
