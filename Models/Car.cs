using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRent.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [DataType(DataType.Text)]
        [Display(Name = "Марка авто")]
        public string? Make { get; set; }

        [Required]
        [MaxLength(30)]
        [DataType(DataType.Text)]
        [Display(Name = "Модель авто")]
        public string? Model { get; set; }

        [Required]
        [Display(Name = "Рік випуску")]
        public int Year { get; set; }

        [Required]
        [MaxLength(30)]
        [DataType(DataType.Text)]
        [Display(Name = "Колір")]
        public string? Color { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Ціна за день")]
        public int DailyRate { get; set; }

        [Required]
        [Display(Name = "В наявності")]
        public bool Available { get; set; }

        [Required]
        [Display(Name = "Фото")]
        public string? PhotoPath { get; set; }
        
        [NotMapped]
        public IFormFile? CarImage { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
