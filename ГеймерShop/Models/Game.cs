using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ГеймерShop.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int PictureId { get; set; }
        public Picture Picture { get; set; }
        public string Name { get; set; }
        public int SystemRequirementsId { get; set; }
        public SystemRequirements SystemRequirements { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int PlaingFieldId { get; set; }
        public PlaingField PlaingField { get; set; }
        public float Price { get; set; }
    }
}
