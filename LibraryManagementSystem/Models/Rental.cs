namespace LibraryManagementSystem.Models
{
    public class Rental : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal TotalCost { get; set; }
    }
}
