using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCPG.Core.Models
{
    public class AuthConfig
    {
        public string ClientId { get; set; }
        public string clientSecret { get; set; }
        public string integrationKey { get; set; }
        public string callBackUrl { get; set; }
        public string merchantCode { get; set; }
    }

      public class PaystackAuthConfig
    {
        public string ClientId { get; set; }
        public string clientSecret { get; set; }
        public string integrationKey { get; set; }
        public string callBackUrl { get; set; }
        public string merchantCode { get; set; }
    }
}