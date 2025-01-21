namespace LibraryManagementSystem.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = new Guid();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
