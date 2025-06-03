using System.ComponentModel.DataAnnotations;

namespace FinacialProjectVersion3.Models.Entity
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(10)]
        public string Type { get; set; }
        public string? IconPath { get; set; }
        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
