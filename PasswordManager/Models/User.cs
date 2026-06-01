using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public long Index { get; set; }
        public User() {}
    }
}
