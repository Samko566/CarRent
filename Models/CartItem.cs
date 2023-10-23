using CarRent.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRent.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CarId { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal Discount { get; set; }
        public decimal Penalty { get; set; }
        public decimal TotalPrice { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public AppUser User { get; set; }
    }
}
