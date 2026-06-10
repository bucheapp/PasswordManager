using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    public class ServiceInfo
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string? Url { get; set; }
        public long DisplayIndex { get; set; }
        public DateTime CreatedAt { get; set; }
        public ServiceInfo() { }
    }
}
