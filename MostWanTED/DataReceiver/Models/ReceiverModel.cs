using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace DataReceiver.Models
{
    public class ReceiverModel
    {
        public string name { get; set; }
        public int minAge { get; set; }
        public int maxAge { get; set; }
        public string type1 { get; set; }
        public string type2 { get; set; }
        public string type3 { get; set; }
        public string type4 { get; set; }
        public double val1 { get; set; }
        public double val2 { get; set; }
        public double val3 { get; set; }
        public double val4 { get; set; }
    }
}
