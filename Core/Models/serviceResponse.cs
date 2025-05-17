using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralPG.Models
{
    public class serviceResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? code { get; set; }
    }
}