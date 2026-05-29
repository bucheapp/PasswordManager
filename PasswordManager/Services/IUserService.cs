using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Models;

namespace PasswordManager.Services
{
    internal interface IUserService
    {
        void Create(User user);
        void Delete(String name);
        void Update(User user);
        List<User> GetAll();
        User Get(long id);
    }
}
