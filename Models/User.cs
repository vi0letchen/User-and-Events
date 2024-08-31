using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class User
    {
        [Key]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Address { get; set; }

    }
}
