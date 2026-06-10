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

    public class AccountInfo
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public AuthType AuthType { get; set; }
        public long DisplayIndex { get; set; }
        public long ServiceInfoId { get; set; }
        public DateTime CreatedAt { get; set; }
        public AccountInfo() {}
    }
}
