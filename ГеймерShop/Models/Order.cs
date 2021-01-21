using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ГеймерShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string CustomerId { get; set; }
        public User Customer { get; set; }
        public string Email { get; set; }
        public DateTime OrderDate { get; set; }

        public List<Key> Keys { get; set; }

        public string Cart { get; set; }

        public Order()
        {
            Keys = new List<Key>();
        }

        public Cart GetCartObject()
        {
            return JsonSerializer.Deserialize<Cart>(Cart);
        }
    }
}
