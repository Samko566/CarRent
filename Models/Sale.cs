using System.ComponentModel.DataAnnotations;

namespace CarRent.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}