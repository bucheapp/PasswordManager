using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    internal class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public int Index { get; set; }
        public User(string name,bool isDefault)
        {
            Name = name;
            IsDefault = isDefault;
        }
    }
}
