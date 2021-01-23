using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ГеймерShop.Models
{
    public class Cart
    {
        public List<CartLine> lineCollection { get; set; }
        public Cart()
        {
            lineCollection = new List<CartLine>();
        }
        public IEnumerable<CartLine> Lines() { return lineCollection; }
        public void AddItem(Game game, int count)
        {
            CartLine line = lineCollection
                .Where(g => g.Game.Id == game.Id)
                .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Game = new ShortGame
                    { Id = game.Id, Name = game.Name, Price = game.Price },
                    Count = count
                });
            }
            else
            {
                line.Count += count;
            }
        }
        public void RemoveItem(Game gadget, int count)
        {
            CartLine line = lineCollection
                .Where(g => g.Game.Id == gadget.Id)
                .FirstOrDefault();
            if (line != null)
            {
                line.Count -= count;
            }
            if (line.Count <= 0)
            {
                lineCollection.Remove(line);
            }
        }

        public float GetSum()
        {
            return lineCollection.Sum(l => l.Game.Price * l.Count);
        }

        public void Clear()
        {
            lineCollection.Clear();
        }

    }

    public class CartLine
    {
        public ShortGame Game { get; set; }
        public int Count { get; set; }
    }

    public class ShortGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }

    }

   
}
