
using WingetNexus.Data.Models;

namespace WingetNexus.Server.Security
{
    public class DatabaseAuthenticationManager : IDatabaseAuthenticationManager
    {
        private readonly IList<ApplicationUser> users = new List<ApplicationUser>
        { 
            new ApplicationUser { UserName= "test1", Role= "User"  },
            new ApplicationUser { UserName= "test2", Role="Administrator" }
        };

        private readonly IDictionary<string, Tuple<string, string>> tokens =
            new Dictionary<string, Tuple<string, string>>();

        public IDictionary<string, Tuple<string, string>> Tokens => tokens;

        public string Authenticate(string username, string password)
        {
            if (!users.Any(u => u.UserName == username))
            {
                return null;
            }

            var token = Guid.NewGuid().ToString();

            tokens.Add(token, new Tuple<string, string>(username,
                users.First(u => u.UserName == username).Role));

            return token;
        }
    }
}
