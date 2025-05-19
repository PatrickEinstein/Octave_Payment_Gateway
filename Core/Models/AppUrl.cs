using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Models
{
    public class AppUrl
    {
        public string BaseUrl { get; set; }
        public string BaseUrl2 { get; set; }
        public string paymentUrlHost { get; set; }
        public string ClientId { get; set; }
        public string clientSecret { get; set; }
        public string integrationKey { get; set; }
        public string notificationUrl { get; set; }
    }

    public class PayStackAppUrls
    {
        public string BaseUrl { get; set; }
        public string BaseUrl2 { get; set; }
        public string paymentUrlHost { get; set; }
        public string ClientId { get; set; }
        public string clientSecret { get; set; }
        public string integrationKey { get; set; }
        public string notificationUrl { get; set; }
    }
    
      public class FlutterWaveAppUrls
    {
        public string BaseUrl { get; set; }
        public string BaseUrl2 { get; set; }
        public string paymentUrlHost { get; set; }
        public string ClientId { get; set; }
        public string clientSecret { get; set; }
        public string integrationKey { get; set; }
        public string notificationUrl { get; set; }
    }

 
}