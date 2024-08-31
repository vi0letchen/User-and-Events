using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Organizer
    {
        [Key]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }

    }
}