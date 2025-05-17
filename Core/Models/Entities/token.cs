using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Core.Models.Entities
{
    public class  AuthTokens
    {
        [Key]
        public int Id { get; set; }
        public string access_token { get; set; } = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkU4NzcxNTMxMzU1MTRGMjhCN0M5NDE1NjY1N0REOEFCRURDODI4M0VSUzI1NiIsInR5cCI6ImF0K2p3dCIsIng1dCI6IjZIY1ZNVFZSVHlpM3lVRldaWDNZcS0zSUtENCJ9.eyJuYmYiOjE3MzAxMjI3OTcsImV4cCI6MTczMDEyNjM5NywiaXNzIjoiaHR0cDovLzE3OC42Mi4xMTYuMjMzIiwiYXVkIjpbInBheW1lbnRhcGkiLCJodHRwOi8vMTc4LjYyLjExNi4yMzMvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6Ik5haTAwMDAzMTkiLCJqdGkiOiIxREExRjNCRjVFQUNGQUM3NDk1MTBEQzBGRkU2QjkwRSIsImlhdCI6MTczMDEyMjc5Nywic2NvcGUiOlsicGF5bWVudGFwaSJdfQ.Xg-gIIPhD-1miirbL4SRRU1Pzfnlm7kZQvSTS770hxvlqPs8E6Qk6yxLT6o2eHSgZc9LpXlNS-1LoSlzcSVXCR43wgqo-Bdjw7RCkf3f4FYhkqdM6hPT4adVSL1D19PLiwf7XwcDW5ECuhTpV8LBO4FoOuLLL0Xx6DB5TsEXzGn-DjU23oT4XzhQlIFtXydUltzLpn_B6-ZKCHcM7eJAR1XcE6C3ri7EUcG45m2_ilNlIUEG9F9o3CEPRvAJr55gMws3XHNBjBolGRMmCTXAUGNEFNpjiMjzEQHNNqDcScdLOWCd0Dau6Qq_iKpR9gSZF7uIxynLhBjmOgzKsTz3gQ";
        public long expires_in { get; set; }
        public string refreshtoken { get; set; }
        public DateTime timeGenerated { get; set; } = DateTime.UtcNow.AddHours(1);
        public bool status { get; set; }
    }
}