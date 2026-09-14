using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomersOrdersMVC.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; } 
        public string CustomerID { get; set; } = default!;
        public Customer Customer { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public decimal Freight { get; set; } = default!;
        public string ShipCity { get; set; } = default!;
        public string ShipCountry { get; set; } = default!;

    }
}