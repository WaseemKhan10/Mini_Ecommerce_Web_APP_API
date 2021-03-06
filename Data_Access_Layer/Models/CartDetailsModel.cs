using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Ecommerce_Comsuming_APIS.Models
{
    public class CartDetailsModel
    {
        [Key]
        public int DetailsID { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        [ForeignKey("CartID")]
        public int CID { get; set; }
        public string UserID { get; set; }
        public decimal LineTotal { get; set; }
    }
}
