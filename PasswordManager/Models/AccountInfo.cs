using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public enum AuthType
    {
        UsernamePassword,
        EmailPassword
    }

    internal class AccountInfo
    {
        public long Id { get; set; }
        public string? Url { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public AuthType AuthType { get; set; }
        public int Index { get; set; }

        public AccountInfo(string? url, string name, string password, AuthType authType)
        {
            Url = url;
            Name = name;
            Password = password;
            AuthType = authType;
        }
    }
}
