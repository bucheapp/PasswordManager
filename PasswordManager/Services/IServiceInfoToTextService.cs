using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Services
{
    public interface IServiceInfoToTextService
    {
        string Convert(List<ServiceInfo> serviceInfos);
    }
}
