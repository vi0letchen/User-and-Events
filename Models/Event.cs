using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Start { get; set; }
        [Required]
        public string End { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Location { get; set; }
    }
}