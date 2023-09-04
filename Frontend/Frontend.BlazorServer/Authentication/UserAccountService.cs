namespace Frontend.BlazorServer.Authentication
{
    public class UserAccountService
    {
        private readonly List<UserAccount> users;

        public UserAccountService()
        {
            users = new List<UserAccount>
            {
                new UserAccount{ UserName = "admin", Password = "admin", Role = "Administrator" },
                new UserAccount{ UserName = "user", Password = "user", Role = "User" }
            };
        }

        public UserAccount? GetByUserName(string userName)
        {
            return users.FirstOrDefault(x => x.UserName == userName);
        }
    }
}
