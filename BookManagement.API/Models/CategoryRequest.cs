using System.ComponentModel.DataAnnotations;

namespace BookManagement.API.Models
{
    public class CategoryRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}