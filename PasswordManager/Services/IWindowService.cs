using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    public interface IWindowService
    {
        SelectUserWindow? ShowSelectUserWindow(List<User> users, User? prevSelectedUser);
        CreateUserWindow? ShowCreateUserWindow(string prevName, string prevPassword);
    }
}
