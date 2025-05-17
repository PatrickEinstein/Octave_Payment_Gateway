using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace CentralPG.Infrasturcture.Interfaces.Utilities
{
    public interface IApiCaller
    {
        Task<string> POST(HttpContent payload, string apiUrl, string token,IDictionary<string, string> headers);
        Task<string> GET(string apiUrl, string token,IDictionary<string, string> headers);
        Task<string> PUT(HttpContent payload, string apiUrl, string token,IDictionary<string, string> headers);
        Task<string> DELETE(string apiUrl, string token,IDictionary<string, string> headers);
    }
}