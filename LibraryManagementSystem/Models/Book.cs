using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book : BaseEntity
    {

        [Required(ErrorMessage = "Kitap adı gereklidir")]
        [Display(Name = "Kitap Adı")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yazar adı gereklidir")]
        [Display(Name = "Yazar")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Günlük kira fiyatı gereklidir.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Günlük kira fiyatı 0'dan büyük olmalıdır.")]
        public decimal DailyRentalPrice { get; set; }

        [Display(Name = "Basım Yılı")]
        public int? PublishYear { get; set; }

        [Required(ErrorMessage = "Kategori seçimi gereklidir")]
        [Display(Name = "Kategori")]
        public Guid CategoryId { get; set; }

        public Category Category { get; set; }

        [Display(Name = "Stok Durumu")]
        public bool IsAvailable { get; set; } = true;
    }
}
