using Authorization.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Models
{
    public class HomePageVM
    {
        public List<Ad> Ads { get; set; }
        public List<int> AdIds {get;set;}
    }
}
