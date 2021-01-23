using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ГеймерShop.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
