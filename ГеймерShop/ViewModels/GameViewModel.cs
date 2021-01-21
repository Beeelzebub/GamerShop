using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ГеймерShop.ViewModels
{
    public class GameViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public string OS { get; set; }

        [Required]
        public int PlaingFieldId { get; set; }

        [Required]
        public string CPU { get; set; }

        [Required]
        public string RAM { get; set; }

        [Required]
        public string GPU { get; set; }

        [Required]
        public string Memory { get; set; }

        [Required]
        public int GenreId { get; set; }
        public IFormFile Image { get; set; }

    }
}
