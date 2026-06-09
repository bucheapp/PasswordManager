using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Models;

namespace PasswordManager.Services
{
    public class ServiceInfoToTextService : IServiceInfoToTextService
    {
        IAccountInfoService _accountInfoService;

        public ServiceInfoToTextService(IAccountInfoService accountInfoService)
        {
            _accountInfoService = accountInfoService;
        }

        public string Convert(List<ServiceInfo> serviceInfos)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ServiceInfo serviceInfo in serviceInfos) {
                string serviceTitle = serviceInfo.Title;
                string? serviceUrl = serviceInfo.Url;
                List<AccountInfo> accountInfos = _accountInfoService.GetByServiceInfoId(serviceInfo.Id);

                if(accountInfos.Count <= 0)
                {
                    continue;
                }

                if (serviceUrl == null)
                {
                    sb.Append($"# {serviceTitle}\n");
                } else
                {
                    sb.Append($"# [{serviceTitle}]({serviceUrl})\n");
                }

                foreach(AccountInfo accountInfo in accountInfos) {
                    string name = accountInfo.Name;
                    string password = accountInfo.Password;

                    sb.Append($"\nName: {name}\n\\");
                    sb.Append($"Password: {password}\n\\");
                }
            }

            return sb.ToString();
        }
    }
}
