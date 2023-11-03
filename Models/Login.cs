using rehome.Enums;
using System.ComponentModel.DataAnnotations;

namespace rehome.Models
{
    public class Login
    {
        [Display(Name = "ログインID")]
        [Required(ErrorMessage = "ログインIDを入力してください")]
        public string? ログインID { get; set; }

        [Display(Name = "パスワード")]
        [Required(ErrorMessage = "パスワードを入力してください")]
        public string? Pass { get; set; }

        //public Role Role { get; set; }
    }
}
