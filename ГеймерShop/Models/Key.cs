using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ГеймерShop.Models
{
    public class Key
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int KeyStatusId { get; set; }
        public KeyStatus KeyStatus { get; set; }
        
    }
}
