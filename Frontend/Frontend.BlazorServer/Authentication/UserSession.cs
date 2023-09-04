namespace Frontend.BlazorServer.Authentication
{
    public class UserSession
    {
        public required string UserName { get; set; }
        public required string Role { get; set; }
    }
}
