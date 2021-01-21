using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ГеймерShop.Models
{
    public class SystemRequirements
    {
        public int Id { get; set; }
        public string OS { get; set; }
        public string CPU { get; set; }
        public string RAM { get; set; }
        public string GPU { get; set; }
        public string Memory { get; set; }
    }
}
