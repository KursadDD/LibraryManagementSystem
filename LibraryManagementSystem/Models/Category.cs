using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Category :  BaseEntity
    {

        [Required(ErrorMessage = "Kategori adı gereklidir")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }
    }
}
