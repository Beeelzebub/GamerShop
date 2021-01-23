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

        public int Id { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Name { get; set; }


        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public float Price { get; set; }

        [Display(Name = "Операционная система")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string OS { get; set; }

        [Display(Name = "Игровая платформа")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int PlaingFieldId { get; set; }

        [Display(Name = "Процессор")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string CPU { get; set; }

        [Display(Name = "Оперативная память")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string RAM { get; set; }

        [Display(Name = "Видеокарта")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string GPU { get; set; }

        [Display(Name = "Память")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Memory { get; set; }

        [Display(Name = "Жанр")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int GenreId { get; set; }

        [Display(Name = "Изображение")]
        public IFormFile Image { get; set; }

    }
}
