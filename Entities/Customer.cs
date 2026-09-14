using System.ComponentModel.DataAnnotations;

namespace CustomersOrdersMVC.Entities
{
    public class Customer
    {
        public string CustomerID { get; set; } = default!;
        public string? CompanyName { get; set; }
        [Display(Name = "Customer Name")]
        public string? ContactName { get; set; }
        [Display(Name = "Customer Title")]
        public string? ContactTitle { get; set; }
        [Display(Name = "Customer Address")]
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }
    }
}