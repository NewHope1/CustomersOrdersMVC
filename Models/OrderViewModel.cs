namespace CustomersOrdersMVC.Models
{
    public class OrderViewModel
    {
        public int OrderID { get; set; } = default!;
        public string CustomerID { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public decimal Freight { get; set; }
        public string ShipCity { get; set; } = default!;
        public string ShipCountry { get; set; } = default!;

    }
}