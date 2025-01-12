namespace BookManagement.API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsActive { get; set; }
    }
}