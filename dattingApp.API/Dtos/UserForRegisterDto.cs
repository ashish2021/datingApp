using System.ComponentModel.DataAnnotations;

namespace dattingApp.API.Dtos
{
    public class UserForRegisterDto
    {
         [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="You Must Specify password bw 4 & 8 charactors")]
        public string Password { get; set; }
    }
}