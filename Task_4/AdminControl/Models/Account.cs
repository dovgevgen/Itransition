using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminControl.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public DateTime LastActivity { get; set; }
        public bool Status { get; set; }
        public string UsedSocialNetwork { get; set; }
        public string ProviderKey { get; set; }
    }
}
